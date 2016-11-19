using System;
using System.ComponentModel;
using System.Reflection;
using wUnit.Util;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace wUnit.Model {
  /// <summary>
  /// Describes a TestMethod, total runtime and if it has succeeded, failed or timedout
  /// Implements INotifyPropertyChanged through the AbstractSynchronizationClass
  /// </summary>
  public class TestInformation : AbstractSynchronizationClass, ITestInformation {
    public event PropertyChangedEventHandler PropertyChanged;

    private bool _called;

    public Attribute Attribute { get; set; }

    private bool _running;

    private readonly IList<ITestRunInformation> _runs = new ObservableCollection<ITestRunInformation>();
    private object _testLock = new object();

    public IList<ITestRunInformation> Runs {
      get {
        lock ( _testLock ) {
          return _runs;
        }
      }
    }

    public MethodInfo TargetMethod {
      get;
      private set;
    }

    public bool Called {
      get {
        return _called;
      }
      set {
        if ( _called == value ) {
          return;
        }
        _called = value;
        RaisePropertyChanged( "Called" );
      }
    }

    public bool Running {
      get {
        return _running;
      }
      set {
        if ( _running == value ) {
          return;
        }
        _running = value;
        RaisePropertyChanged( "Running" );
      }
    }

    protected virtual void RaisePropertyChanged( string propertyName ) {
      var local = PropertyChanged;
      if ( local != null ) {
        Fire( (s ) => {
          local.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        } );
      }
    }

    protected virtual void HandleChildPropertyChanged( object sender, PropertyChangedEventArgs e ) {
      RaisePropertyChanged( "" );
    }

    public TestInformation( MethodInfo targetMethod, Attribute attribute = null ) {
      TargetMethod = targetMethod;
      Attribute = attribute;
      ( (INotifyCollectionChanged)Runs ).CollectionChanged += (sender, e ) => {
        if ( e.OldItems != null ) {
          foreach (var item in e.OldItems) {
            var propc = item as INotifyPropertyChanged;
            if ( propc == null ) {
              continue;
            }
            propc.PropertyChanged -= HandleChildPropertyChanged;
          }
        }
        if ( e.NewItems != null ) {
          foreach (var item in e.NewItems) {
            var propc = item as INotifyPropertyChanged;
            if ( propc == null ) {
              continue;
            }
            propc.PropertyChanged += HandleChildPropertyChanged;
          }
        }
      };
      ;
    }
  }
}

