using System;
using System.Linq;
using wUnit.Export;
using wUnit.Util;
using System.Collections.Generic;
using wUnit.Model;

namespace RunTest.Export {
  public class ConsoleExporter : IConfigurationResultExporter {
    public int Export( IContextRunnerConfiguration context, string fileName ) {
      int errorCount = 0;
      string lastConfigurationName = string.Empty;
      var list = GetAllRunInformation( context.Configurations ).OrderBy( i => i.Item3.Start ).ToList();

      foreach (var item in list) {
        if ( lastConfigurationName != item.Item1.Type.Name ) {
          lastConfigurationName = item.Item1.Type.Name;
          if ( item.Item1.Failed > 0 ) {
            WriteError( "'{0}' has not completed completely, failed {1}", lastConfigurationName, item.Item1.Failed, item.Item1.Completed );
          } else {
            WriteInfo( "'{0}' completed {1} test{2}", lastConfigurationName, item.Item1.Tests.Count, item.Item1.Tests.Count > 0 ? "s" : "" );
          }
        }
        if ( item.Item3.State == ResultState.SUCCEEDED ) {
          WriteSuccess( "{0}.{1} completed in {2}", lastConfigurationName, item.Item2.TargetMethod.Name, item.Item3.End.Subtract( item.Item3.Start ) );
        } else if ( item.Item3.State == ResultState.INACTIVE ) {
          WriteInfo( "{0}.{1} is marked as inactive", lastConfigurationName, item.Item2.TargetMethod.Name );
        } else {
          errorCount++;
          WriteError( "{0}.{1} exited with stated {2}", lastConfigurationName, item.Item2.TargetMethod.Name, item.Item3.State );
        }
      }

      return errorCount;
    }

    private Tuple<ConsoleColor,ConsoleColor> SwapColors( ConsoleColor foreground, ConsoleColor background ) {
      var t = new Tuple<ConsoleColor, ConsoleColor>( Console.ForegroundColor, Console.BackgroundColor );
      Console.ForegroundColor = foreground;
      Console.BackgroundColor = background;
      return t;
    }

    private void SwapReset( Tuple<ConsoleColor, ConsoleColor> backup ) {
      SwapColors( backup.Item1, backup.Item2 );
    }

    private void WriteError( string message, params object[] obj ) {
      var conf = SwapColors( ConsoleColor.Red, ConsoleColor.Black );
      Console.Error.WriteLine( "ERROR\t" + message, obj );
      SwapReset( conf );
    }

    private void WriteInfo( string message, params object[] obj ) {
      var conf = SwapColors( ConsoleColor.White, ConsoleColor.Black );
      Console.Error.WriteLine( "INFO\t" + message, obj );
      SwapReset( conf );
    }

    private void WriteSuccess( string message, params object[] obj ) {
      var conf = SwapColors( ConsoleColor.Green, ConsoleColor.Black );
      Console.Error.WriteLine( "SUCCESS\t" + message, obj );
      SwapReset( conf );
    }

    private IEnumerable<Tuple<ITestConfiguration, ITestInformation, ITestRunInformation>> GetAllRunInformation( IEnumerable<ITestConfiguration> configurations ) {
      if ( configurations == null ) {
        yield break;
      }
      foreach (var configuration in configurations) {
        foreach (var test in configuration.Tests) {
          foreach (var run in test.Runs) {
            yield return new Tuple<ITestConfiguration, ITestInformation, ITestRunInformation>( configuration, test, run );
          }
        }
      }
    }

    public string Name { get { return "console"; } }

    public string Description { get { return "Export the results to the console window"; } }

    private readonly Version _version = new Version( 1, 0 );

    public Version Version { get { return _version; } }

    public string DefaultFileNamePattern { get { return "%date%-%state%-%assembly%"; } }

    public string FileExtension { get { return "xml"; } }

    public bool TriggerApplicationExitCode { get { return true; } }
  }
}

