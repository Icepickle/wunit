using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using wUnit.Util;

namespace wUnit.Model {
  /// <summary>
  /// Describes a TestEnvironment by Type and found tests
  /// Can indicate if it is Completed and how many Failed tests methods have happened
  /// Implements INotifyPropertyChanged through the AbstractSynchronizationClass
  /// </summary>
  public class TestConfiguration : AbstractSynchronizationClass, ITestConfiguration, IDisposable {
    public event PropertyChangedEventHandler PropertyChanged;

    private readonly IDictionary<Type, Type> _definedTypes = new Dictionary<Type, Type>();

    public IDictionary<Type, Type> DefinedTypes {
      get {
        return _definedTypes;
      }
    }

    private IContextRunnerConfiguration _contextRunnerConfiguration;

    public IContextRunnerConfiguration ContextRunnerConfiguration {
      get {
        return _contextRunnerConfiguration;
      }
    }

    private Type _type;

    public Type Type {
      get {
        return _type;
      }
      set {
        if ( System.Type.Equals( _type, value ) ) {
          return;
        }
        _type = value;
        RaisePropertyChanged( "Type" );
      }
    }

    private int _failed;

    public int Failed {
      get {
        return _failed;
      }
      set {
        if ( _failed == value ) {
          return;
        }
        _failed = value;
        RaisePropertyChanged( "Failed" );
      }
    }

    private object testLock = new object();
    private readonly IList<ITestInformation> _tests;

    public IList<ITestInformation> Tests {
      get {
        lock ( testLock ) {
          return _tests;
        }
      }
    }

    private bool _completed;

    public bool Completed {
      get {
        return _completed;
      }
      set {
        if ( _completed == value ) {
          return;
        }
        _completed = value;
        RaisePropertyChanged( "Completed" );
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

    public TestConfiguration( IContextRunnerConfiguration configuration, Type type = null ) {
      Completed = false;
      _tests = new ObservableCollection<ITestInformation>();
      _contextRunnerConfiguration = configuration;
      Type = type;
      RegisterToCollection( Tests as INotifyCollectionChanged, TestsCollectionChanged, TestInformationChanged );
    }

    protected virtual void UnregisterFromChild( INotifyPropertyChanged item, PropertyChangedEventHandler handler ) {
      if ( item == null ) {
        return;
      }
      item.PropertyChanged -= handler;
    }

    protected virtual void RegisterToChild( INotifyPropertyChanged item, PropertyChangedEventHandler handler ) {
      if ( item == null ) {
        return;
      }
      item.PropertyChanged += handler;
    }

    protected virtual void UnregisterFromCollection( INotifyCollectionChanged source, NotifyCollectionChangedEventHandler handler, PropertyChangedEventHandler childHandler ) {
      source.CollectionChanged -= handler;
      foreach (var item in (source as IEnumerable)) {
        UnregisterFromChild( item as INotifyPropertyChanged, childHandler );
        if ( item is IDisposable ) {
          ( (IDisposable)item ).Dispose();
        }
      }
    }

    protected virtual void RegisterToCollection( INotifyCollectionChanged source, NotifyCollectionChangedEventHandler handler, PropertyChangedEventHandler childHandler ) {
      foreach (var item in (source as IEnumerable)) {
        RegisterToChild( item as INotifyPropertyChanged, childHandler );
      }
      source.CollectionChanged += handler;
    }

    protected virtual void TestsCollectionChanged( object sender, NotifyCollectionChangedEventArgs e ) {
      if ( e.OldItems != null ) {
        foreach (var item in e.OldItems) {
          UnregisterFromChild( item as INotifyPropertyChanged, TestInformationChanged );
        }
      }
      if ( e.NewItems != null ) {
        foreach (var item in e.NewItems) {
          RegisterToChild( item as INotifyPropertyChanged, TestInformationChanged );
        }
      }
    }

    protected virtual void TestInformationChanged( object sender, PropertyChangedEventArgs e ) {
      var info = sender as ITestInformation;
      if ( info == null ) {
        return;
      }
      if ( string.IsNullOrWhiteSpace( e.PropertyName ) || string.Equals( "Running", e.PropertyName ) ) {
        if ( Tests.All( item => item.Runs.All( run => run.End != DateTime.MinValue ) ) ) {
          Completed = true;
          Failed = Tests.Count( item => item.Runs.Count( run => run.State == ResultState.FAILED || run.State == ResultState.TIMEDOUT || !run.Completed ) > 0 );
        }
      }
    }

    protected override void Dispose( bool disposing ) {
      base.Dispose( disposing );
      if ( disposing ) {
        UnregisterFromCollection( Tests as INotifyCollectionChanged, TestsCollectionChanged, TestInformationChanged );
        Tests.Clear();
        foreach (var item in DefinedTypes) {
          if ( item.Value is IDisposable ) {
            ( (IDisposable)item.Value ).Dispose();
          }
        }
        DefinedTypes.Clear();
      }
    }
  }
}

