using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using wUnit.Util;

/*
 * TODO
 * - Create configuration file to make tests based on a configuration instead of hardcoded settings (after splitting analyzer and tester)
*/
using wUnit.Util.Context;
using System.ComponentModel;
using wUnit.Model;
using System.Threading;
using wUnit.Export;
using RunTest.Export;


namespace RunTest {
  class MainClass {
    #region Static members

    private static ManualResetEvent configurationThrough = new ManualResetEvent( false );
    private static ManualResetEvent applicationCompleted = new ManualResetEvent( false );
    private static readonly List<string> unfoundFileList = new List<string>();
    private static readonly List<string> reflectionTypeLoadExceptionList = new List<string>();

    #endregion

    #region Helper methods for runtime

    private static void RunTimeErrorOccured( object sender, TestErrorOccuredEventArgs e ) {
      if ( e.TestConfiguration != null && e.TestInformation != null ) {
        if ( !string.IsNullOrWhiteSpace( e.Exception.Message ) ) {
          Console.WriteLine( "{0}\r\n{1}", e.Exception, e.Exception.Message );
        }
        Console.WriteLine( "{3}: Error occured for {0} running {1} with exception {2}", e.TestConfiguration.Type.FullName, e.TestInformation.TargetMethod.Name, e.Exception, e.ResultState );
      } else if ( e.TestConfiguration != null ) {
        Console.WriteLine( "{2}: Error occured for {0} with exception {1}", e.TestConfiguration.Type.FullName, e.Exception, e.ResultState );
      } else if ( e.Exception is ReflectionTypeLoadException ) {
        var tle = e.Exception as ReflectionTypeLoadException;
        foreach (var le in tle.LoaderExceptions) {
          Console.WriteLine( "Error loading assembly due to {0}", le.Message );
          if ( !reflectionTypeLoadExceptionList.Contains( le.Message ) ) {
            reflectionTypeLoadExceptionList.Add( le.Message );
          }
        }
      } else if ( e.Exception is FileNotFoundException ) {
        var fne = e.Exception as FileNotFoundException;
        Console.WriteLine( "Cannot find file {0}", fne.FileName );
        if ( !unfoundFileList.Contains( fne.FileName ) ) {
          unfoundFileList.Add( fne.FileName );
        }
      } else {
        Console.WriteLine( "{0}", e.ResultState );
      }
    }

    private static void RunTimeCompleted( object sender, EventArgs e ) {
      configurationThrough.Set();
    }

    #endregion

    #region Print results

    private static int AnalyzeFailuresAndPrintResults( IContextRunnerConfiguration configuration, ProgramOptions options ) {
      int result = 0;
      try {
        result = GetExitCodeFromExporter( options, configuration );
      } catch ( Exception ex ) {
        result = -1;
        Console.WriteLine( "Error occured in exporter\r\n{0}\r\n{1}", ex.Message, ex.StackTrace );
      } finally {
        // cleanup
        foreach (var item in configuration.Configurations) {
          var disposable = item as IDisposable;
          if ( disposable != null ) {
            disposable.Dispose();
          }
        }
      }
      return result;
    }

    #endregion

    #region Export

    private static IConfigurationResultExporter GetExporter( ProgramOptions options ) {
      return new ConsoleExporter();
    }

    private static int GetExitCodeFromExporter( ProgramOptions options, IContextRunnerConfiguration context ) {
      var exporter = GetExporter( options );
      int exporterResult = exporter.Export( context, ExportFilenameHelper.GetFilenameForExportConfiguration( exporter, string.Empty, options ) );
      return exporter.TriggerApplicationExitCode ? exporterResult : 0;
        
    }

    #endregion

    #region Main

    public static void Main( string[] args ) {
      int nrOfErrors = 0;
      #if DEBUG
      if ( args == null || args.Length == 0 ) {
        args = new [] { "-exe" };
      }
      #endif
      var options = ProgramOptions.Parse( args );
      if ( options != null ) {
        System.Diagnostics.Debug.WriteLine( "Current application thread: ", System.Threading.Thread.CurrentThread.ManagedThreadId );
        string currentPath = Environment.CurrentDirectory;
        IContextAnalyzer contextAnalyzer = new ContextAnalyzer();
        using ( IContextRunner runner = new TestSuiteContextRunner() ) {
          runner.ErrorOccured += RunTimeErrorOccured;
          runner.Completed += RunTimeCompleted;
        
          var configuration = ProgramOptions.GetConfigurationForContext( options );
          Console.WriteLine( "Analyzing" );
          contextAnalyzer.CreateConfiguration( configuration, () => {
            try {
              if ( configuration.Configurations != null && configuration.Configurations.Count > 0 ) {
                Console.WriteLine( "Running tests" );
                runner.RunAsync( configuration, configurationThrough );
                configurationThrough.WaitOne();
                nrOfErrors = AnalyzeFailuresAndPrintResults( configuration, options );
              } else {
                Console.WriteLine( "No test methods in referenced assemblies!" );
                nrOfErrors = 1;
              }
              ( configuration as IDisposable ).Dispose();
              Console.WriteLine( "After dispose" );
            } finally {
              applicationCompleted.Set();
            }
          } );
          applicationCompleted.WaitOne();

          if ( unfoundFileList.Count > 0 || reflectionTypeLoadExceptionList.Count > 0 ) {
            Console.WriteLine( "Couldn't find {0} dlls and {1} dlls threw load exceptions", unfoundFileList.Count, reflectionTypeLoadExceptionList.Count );
          }
          Console.WriteLine( "Completed" );
          if ( configuration is IDisposable ) {
            try {
              ( configuration as IDisposable ).Dispose();
            } catch ( Exception ex ) {
              Console.WriteLine( "Couldn't dispose configuration due to: " + ex.Message );
            } finally {
            }
          }
          runner.ErrorOccured -= RunTimeErrorOccured;
          runner.Completed -= RunTimeCompleted;
        }
      }
      Environment.Exit( nrOfErrors );
    }

    #endregion
  }
}
