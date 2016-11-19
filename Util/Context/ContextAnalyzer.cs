using System;
using wUnit.Model;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using wUnit.Attributes;

namespace wUnit.Util.Context {
  public class ContextAnalyzer : AbstractSynchronizationClass, IContextAnalyzer {
    public event EventHandler<ErrorEventArgs> ErrorOccured;

    protected virtual void ReportError( ITestConfiguration configuration, ITestInformation information, Exception ex = null, ResultState state = ResultState.FAILED ) {
      FireDelegate( ErrorOccured, new TestErrorOccuredEventArgs( configuration, information, ex, state ) );
    }

    protected string GetPathFromAssembly( Assembly assembly ) {
      string codeBase = assembly.CodeBase;
      UriBuilder uri = new UriBuilder( codeBase );
      string path = Uri.UnescapeDataString( uri.Path );
      return Path.GetDirectoryName( path );
    }

    protected  string GetPathFromAssemblyName( AssemblyName assemblyName ) {
      string codeBase = assemblyName.CodeBase;
      if ( string.IsNullOrWhiteSpace( codeBase ) ) {
        return Environment.CurrentDirectory;
      }
      UriBuilder uri = new UriBuilder( codeBase );
      if ( string.IsNullOrWhiteSpace( uri.Path ) ) {
        return Environment.CurrentDirectory;
      }
      string path = Uri.UnescapeDataString( uri.Path );
      return Path.GetDirectoryName( path );
    }

    protected void SetCurrentAssemblyDirectory( Assembly assembly ) {
      string oldPath = Environment.CurrentDirectory;
      string newPath = GetPathFromAssembly( assembly );
      if ( !string.Equals( oldPath, newPath ) ) {
        Environment.CurrentDirectory = newPath;
      }
    }

    protected void SetCurrentAssemblyNameDirectory( AssemblyName assemblyName ) {
      string oldPath = Environment.CurrentDirectory;
      string newPath = GetPathFromAssemblyName( assemblyName );
      if ( !string.Equals( oldPath, newPath ) ) {
        Environment.CurrentDirectory = newPath;
      }
    }

    protected void LoadAllReferencedAssemblies( Assembly assembly, ISet<AssemblyName> loadedAssemblies, IContextRunnerConfiguration configuration ) {
      loadedAssemblies.Add( assembly.GetName() );
      SetCurrentAssemblyDirectory( assembly );
      var referenced = assembly.GetReferencedAssemblies().Where( ass => !loadedAssemblies.Contains( ass ) );
      // loads all referenced assemblies
      foreach (var referencedAssemblyName in referenced) {
        if ( loadedAssemblies.Contains( referencedAssemblyName ) ) {
          continue;
        }
        try {
          SetCurrentAssemblyNameDirectory( referencedAssemblyName );
          LoadAllReferencedAssemblies( configuration.ContextAppDomain.Load( referencedAssemblyName ), loadedAssemblies, configuration );
        } catch ( Exception ex ) {
          ReportError( null, null, ex );
        }
      }
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

    protected Type[] GetTypesFromAssembly( Assembly assembly, ISet<AssemblyName> loadedAssemblies, IContextRunnerConfiguration configuration ) {
      var result = new List<Type>();
      try {
        SetCurrentAssemblyDirectory( assembly );
        LoadAllReferencedAssemblies( assembly, loadedAssemblies, configuration );
        var types = assembly.GetTypes();
        foreach (var type in types) {
          var attr = type.GetCustomAttribute<TestEnvironmentAttribute>();
          if ( attr == null || !attr.IsEnabled ) {
            continue;
          }
          result.Add( type );
        }
        loadedAssemblies.Add( assembly.GetName() );
      } catch ( ReflectionTypeLoadException ex ) {
        if ( ex.LoaderExceptions != null ) {
          ReportError( null, null, ex );
        }
        result.AddRange( ex.Types.Where( type => type != null ) );
      }
      return result.ToArray();
    }

    protected virtual void AnalyzeConfiguration( IContextRunnerConfiguration configuration ) {
      var assemblies = configuration.ContextAppDomain.GetAssemblies();
      if ( assemblies.Length == 0 ) {
        return;
      }
      double step = 100.0 / assemblies.Length;
      double current = 0;
      var loadedAssemblies = new HashSet<AssemblyName>( new AssemblyNameComparer() );
      foreach (var assembly in assemblies) {
        var testTypeClasses = GetTypesFromAssembly( assembly, loadedAssemblies, configuration );
        if ( testTypeClasses.Length == 0 ) {
          current += step;
          continue;
        }
        foreach (var type in testTypeClasses) {
          var methods = type.GetMethods( BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
          var tInfo = new TestConfiguration( configuration, type );
          var definedImplementations = type.GetCustomAttributes<DefineImplementationAttribute>();
          if ( definedImplementations != null ) {
            foreach (var di in definedImplementations) {
              tInfo.DefinedTypes.Add( di.InterfaceType, di.ImplementationType );
            }
          }
          foreach (var method in methods) {
            CheckAndAddMethodMatchingAttribute<AbstractActivatorAttribute>( method, tInfo.Tests );
          }
          if ( tInfo.Tests.Count == 0 ) {
            continue;
          }
          configuration.Configurations.Add( tInfo );
        }
        current += step;
      }
    }

    public void CreateConfiguration( IContextRunnerConfiguration configuration, Action callback ) {
      Thread configurationThread = new Thread( () => {
        try {
          Console.WriteLine( "Inside analysis" );
          AnalyzeConfiguration( configuration );
        } catch ( Exception ex ) {
          Console.WriteLine( ex.Message );
        } finally {
          callback();
        }
      } );
      Console.WriteLine( "Starting configurationThread" );
      configurationThread.Start();
      Console.WriteLine( "Calling callback" );
    }

    public ContextAnalyzer() {
    }
  }
}

