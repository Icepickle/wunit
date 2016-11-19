using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using wUnit.Model;
using System;
using System.Security.Policy;

namespace wUnit.Util {
  [Serializable]
  public class ContextConfiguration: IContextRunnerConfiguration, IDisposable {
    private AppDomain _configurationAppDomain;
    private readonly object _appDomainLock = new object();

    public AppDomain ContextAppDomain {
      get {
        EnsureDomainCreated();
        return _configurationAppDomain;
      }
    }

    public Assembly[] TargetAssemblies {
      get;
      private set;
    }

    public IList<ITestConfiguration> Configurations {
      get;
      private set;
    }

    protected virtual void AssemblyLoaded( object sender, AssemblyLoadEventArgs e ) {
      var oldColor = Console.ForegroundColor;
      Console.ForegroundColor = ConsoleColor.Blue;
      Console.WriteLine( "Loaded {0} succesfully", e.LoadedAssembly.FullName );
      Console.ForegroundColor = oldColor;
    }

    protected void EnsureDomainCreated() {
      if ( _configurationAppDomain != null ) {
        return;
      }
      lock ( _appDomainLock ) {
        if ( _configurationAppDomain != null ) {
          return;
        }
        Evidence evidence = AppDomain.CurrentDomain.Evidence;
        _configurationAppDomain = AppDomain.CreateDomain( "ConfigurationAppDomain", evidence, Environment.CurrentDirectory, ".", true );
        _configurationAppDomain.AssemblyLoad += AssemblyLoaded;
      }
    }

    public static IContextRunnerConfiguration FromDirectory( string directory, bool includeExe = false ) {
      return FromDirectories( includeExe, directory );
    }

    public static IContextRunnerConfiguration FromDirectories( bool includeExe = false, params string[] directories ) {
      string[] files;
      var cc = new ContextConfiguration();
      foreach (var directory in directories) {
        if ( includeExe ) {
          files = Directory.GetFiles( directory, "*.exe" );
          foreach (var file in files) {
            cc.ContextAppDomain.Load( Assembly.LoadFrom( file ).GetName() );
          }
        }
        files = Directory.GetFiles( directory, "*.dll" );
        foreach (var file in files) {
          /*string filename = Path.GetFileName( file );
          if ( String.Equals( filename, "wunit.dll", StringComparison.OrdinalIgnoreCase ) ) {
            Console.WriteLine( "Skipping wUnit" );
            continue;
          }*/
          cc.ContextAppDomain.Load( Assembly.LoadFrom( file ).GetName() );
        }
      }
      return cc;
    }

    protected ContextConfiguration() {
      EnsureDomainCreated();
      Configurations = new ObservableCollection<ITestConfiguration>();
    }

    public ContextConfiguration( string assemblyName )
      : this( Assembly.LoadFrom( assemblyName ) ) {
      // intended blank
    }

    public ContextConfiguration( Assembly assembly )
      : this( new[] { assembly } ) {
      // intended blank
    }

    public ContextConfiguration( Assembly[] assemblies )
      : this() {
      foreach (var assembly in assemblies) {
        ContextAppDomain.Load( assembly.GetName() );
      }
    }

    private bool _isDisposed = false;

    protected virtual void Dispose( bool disposing ) {
      if ( _isDisposed ) {
        return;
      }
      if ( disposing ) {
        _isDisposed = true;
        if ( _configurationAppDomain != null ) {
          //_configurationAppDomain.AssemblyLoad -= AssemblyLoaded;
          AppDomain.Unload( _configurationAppDomain );
          _configurationAppDomain = null;
        }
      }
    }

    public void Dispose() {
      Dispose( true );
    }
  }
}

