using System;
using wUnit.Util;

namespace RunTest {
  public class ProgramOptions {
    public string Directory { get; set; }

    public bool IsDirectory {
      get {
        return !IsFile;
      }
    }

    public string File { get; set; }

    public bool IsFile {
      get {
        return !string.IsNullOrWhiteSpace( File );
      }
    }

    public bool IncludeExecutables { get; set; }

    public bool FailWhenNoTestsWereDetected { get; set; }

    public ProgramOptions() {
      Directory = Environment.CurrentDirectory;
      File = string.Empty;
      IncludeExecutables = false;
      FailWhenNoTestsWereDetected = false;
    }

    public static IContextRunnerConfiguration GetConfigurationForContext( ProgramOptions options ) {
      if ( options.IsDirectory ) {
        return ContextConfiguration.FromDirectory( string.IsNullOrWhiteSpace( options.Directory ) ? Environment.CurrentDirectory : options.Directory, options.IncludeExecutables );
      } else {
        return new ContextConfiguration( options.File );
      }
    }

    public static void PrintHelp() {
      Console.WriteLine( @"WUnit RunTest program
Usage: 
  runtest [-exe] [--help] [-?] [/?] [-s] [--strict]
  runtest -d:directory [-exe] [-s] [--strict]
  runtest -f:file [-s] [--strict]

Params:
  -d:directory  Scans this directory for any wUnit compatible files
  -exe          In case you are scanning a directory, indicates if you 
                want to include exe files
  -f:file       Scan a specific file
  -? --help /?  Ignores all other parameters and prints this help
  -s --strict   Indicates that at least 1 test class must be found otherwise it fails

When it is run without parameters, it will skim the current directory without exe files" );
    }

    public static ProgramOptions Parse( string[] arguments ) {
      var options = new ProgramOptions();
      bool dirset = false;
      bool fileset = false;
      foreach (string argument in arguments) {
        if ( argument == "/?" ||
             string.Equals( argument, "--help", StringComparison.CurrentCultureIgnoreCase ) ||
             argument == "-?" ) {
          ProgramOptions.PrintHelp();
          return null;
        }
        if ( argument.StartsWith( "-d:", StringComparison.CurrentCultureIgnoreCase ) ) {
          dirset = true;
          options.Directory = argument.Substring( argument.IndexOf( ":" ) + 1 );
          continue;
        }
        if ( string.Equals( argument, "-exe", StringComparison.CurrentCultureIgnoreCase ) ) {
          options.IncludeExecutables = true;
          continue;
        }
        if ( argument.StartsWith( "-f:", StringComparison.CurrentCultureIgnoreCase ) ) {
          fileset = true;
          options.File = argument.Substring( argument.IndexOf( ":" ) + 1 );
          continue;
        }
        if ( string.Equals( argument, "--strict", StringComparison.CurrentCultureIgnoreCase ) ||
             string.Equals( argument, "-s", StringComparison.OrdinalIgnoreCase ) ) {
          options.FailWhenNoTestsWereDetected = true;
        }
      }
      if ( dirset && fileset ) {
        Console.Error.WriteLine( "Cannot choose both file and directory to run the test configuration, choose 1" );
        return null;
      }
      return options;
    }
  }
}

