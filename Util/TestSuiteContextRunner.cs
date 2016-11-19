using System;
using System.Linq;
using System.Threading;
using System.ComponentModel;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

using wUnit.Model;
using wUnit.Attributes;
using wUnit.Util.Factory;
using wUnit.Util.Cache;
using System.Diagnostics;

namespace wUnit.Util {
  public class TestSuiteContextRunner : AbstractSynchronizationClass, IContextRunner {
    public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

    public event EventHandler<TestErrorOccuredEventArgs> ErrorOccured;

    public event EventHandler<RunWorkerCompletedEventArgs> Completed;

    protected virtual void ReportProgress( int progress, string text ) {
      FireDelegate( ProgressChanged, new ProgressChangedEventArgs( progress, text ) );
    }

    protected virtual void ReportError( ITestConfiguration configuration, ITestInformation information, Exception ex = null, ResultState state = ResultState.FAILED ) {
      FireDelegate( ErrorOccured, new TestErrorOccuredEventArgs( configuration, information, ex, state ) );
    }

    protected virtual void Run( IContextRunnerConfiguration configuration ) {
      double current = 0;
      ReportProgress( (int)current, "Starting testconfiguration" );
      if ( configuration.Configurations.Count == 0 ) {
        ReportError( null, null, new InvalidOperationException( "Cannot run a non initialized configuration!" ), ResultState.INACTIVE );
        return;
      }
      double step = 100.0 / configuration.Configurations.Count;
      foreach (var test in configuration.Configurations) {
        ReportProgress( (int)current, string.Format( "Running {0}", test.Type.FullName ) );
        try {
          RunTest( test );
        } catch ( Exception ex ) {
          ReportError( test, null, ex );
        } finally {
          ClearCache();
        }
        current += step;
      }
      ReportProgress( (int)current, "Done" );
      FireDelegate( Completed, null );
    }

    protected virtual void CheckAndAddMethodMatchingAttribute<T>( MethodInfo method, IList<ITestInformation> list )
			where T: Attribute {
      var mInfo = method.GetCustomAttribute<T>();
      if ( mInfo != null ) {
        if ( !( mInfo is AbstractActivatorAttribute ) || ( mInfo as AbstractActivatorAttribute ).IsEnabled ) {
          list.Add( new TestInformation( method, mInfo ) );
        }
      }
    }

    protected virtual void ClearCache() {
      // clear the cache used by the EntityFactory (happens inbetween testing)
      CacheSettingsHelper<Type, object>.Current.Clear();
    }

    protected virtual void RunTest( ITestConfiguration configuration ) {
      if ( configuration == null ) {
        return;
      }
      var instance = EntityFactory.Factory.ConstructType( configuration.Type, configuration );
      var maxTime = configuration.Type.GetCustomAttribute<MaxTimeAttribute>();
      if ( maxTime == null || !maxTime.IsEnabled ) {
        RunUnlimitedTest( configuration, instance );
      } else if ( !RanWithinTimeConstraint( configuration, instance, maxTime ) ) {
        ReportError( configuration, null, null, ResultState.TIMEDOUT );
      }
      var disposable = instance as IDisposable;
      if ( disposable != null ) {
        disposable.Dispose();
      }
    }

    protected  virtual void RunUnlimitedTest( ITestConfiguration configuration, object instance ) {
      RunTestInformationAs<InitializeTestAttribute>( instance, configuration.Tests, configuration );
      RunTestInformationAs<RunTestAttribute>( instance, configuration.Tests, configuration );
      RunTestInformationAs<DestroyTestAttribute>( instance, configuration.Tests, configuration );
    }

    protected virtual bool RanWithinTimeConstraint( ITestConfiguration configuration, object instance, MaxTimeAttribute mta ) {
      ManualResetEvent mre = new ManualResetEvent( false );
      var method = new Action( () => {
        RunUnlimitedTest( configuration, instance );
      } );
      Thread activatorInstance = new Thread( () => {
        var iar = method.BeginInvoke( null, null );
        method.EndInvoke( iar );
        mre.Set();
      } );
      activatorInstance.Start();
      var result = mre.WaitOne( mta.TimeOutAfter );
      if ( activatorInstance.IsAlive ) {
        activatorInstance.Abort();
        result = false;
      }
      return result;
    }

    protected virtual bool RunWithTimeoutSucceeded( ITestInformation testInformation, int maxWaitTime, object targetObject, params object[] arguments ) {
      ManualResetEvent mre = new ManualResetEvent( false );
      var method = new Action( () => {
        testInformation.TargetMethod.Invoke( targetObject, arguments );
      } );
      Thread activatorInstance = new Thread( () => {
        var iar = method.BeginInvoke( null, null );
        method.EndInvoke( iar );
        mre.Set();
      } );
      activatorInstance.Start();
      var result = mre.WaitOne( maxWaitTime );
      if ( activatorInstance.IsAlive ) {
        activatorInstance.Abort();
      }
      return result;
    }

    protected virtual bool RunSucceeded( ITestInformation testInformation, object targetObject, params object[] arguments ) {
      testInformation.TargetMethod.Invoke( targetObject, arguments );
      return true;
    }

    protected virtual object[] BuildArguments( MethodInfo method, ITestConfiguration configuration ) {
      return EntityFactory.Factory.FillParamInfo( method.GetParameters(), configuration );
    }

    protected virtual void RunTestInformationAs<T>( object targetObject, IList<ITestInformation> source, ITestConfiguration configuration ) where T: Attribute {
      var initializers = source
				.Where( info => info.Attribute is T )
				.OrderBy( info => info.Attribute is AbstractSequencedAttribute ? ( (AbstractSequencedAttribute)info.Attribute ).SequenceNumber : 0 );
      foreach (ITestInformation init in initializers) {
        var testResult = new TestRunInformation();
        try {
          if ( typeof(T).Equals( typeof(RunTestAttribute) ) ) {
            RunTestInformationAs<PreTestAttribute>( targetObject, source, configuration );
          }
          var runTime = init.TargetMethod.GetCustomAttribute<MaxTimeAttribute>();
          init.Runs.Add( testResult );
          init.Called = true;
          testResult.Called = true;
          testResult.Running = true;
          testResult.Start = DateTime.Now;
          testResult.State = ResultState.RUNNING;
          var arguments = BuildArguments( init.TargetMethod, configuration );
          if ( runTime != null && runTime.IsEnabled ) {
            testResult.Completed = RunWithTimeoutSucceeded( init, runTime.TimeOutAfter, targetObject, arguments );
            if ( !testResult.Completed ) {
              testResult.State = ResultState.TIMEDOUT;
              ReportError( configuration, init, null, testResult.State );
              continue;
            }
          } else {
            testResult.Completed = RunSucceeded( init, targetObject, arguments );
          }
          testResult.State = ResultState.SUCCEEDED;
          if ( typeof(T).Equals( typeof(RunTestAttribute) ) ) {
            RunTestInformationAs<PostTestAttribute>( targetObject, source, configuration );
          }
        } catch ( Exception ex ) {
          testResult.State = ResultState.FAILED;
          ReportError( configuration, init, ex, testResult.State );
        } finally {
          testResult.End = DateTime.Now;
          testResult.Running = false;
        }
      }
    }

    private bool _running = false;
    private Thread _workThread;
    private readonly object _threadLock = new object();

    public void RunAsync( IContextRunnerConfiguration configuration, ManualResetEvent mre ) {
      if ( _running ) {
        mre.Set();
        return;
      }
      lock ( _threadLock ) {
        if ( _running ) {
          mre.Set();
          return;
        }
        _running = true;
      }
      _workThread = new Thread( obj => {
        try {
          Console.WriteLine( "Starting worker thread" );
          Run( (IContextRunnerConfiguration)obj );
          _running = false;
          _workThread = null;
        } finally {
          mre.Set();
        }
      } );
      _workThread.Start( configuration );
    }

    private bool _isDisposed = false;

    protected override void Dispose( bool disposing ) {
      base.Dispose( disposing );
      if ( disposing ) {
        if ( !_isDisposed ) {
          _isDisposed = true;
          ClearCache();
        }
      }
    }
  }
}