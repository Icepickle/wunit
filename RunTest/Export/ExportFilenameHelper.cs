using System;
using wUnit.Export;
using System.IO;
using wUnit.Util;

namespace RunTest {
  public static class ExportFilenameHelper {
    public static string GetFilenameForExportConfiguration( IConfigurationResultExporter exporter, string filename, ProgramOptions options ) {
      string optionsFilename = options.IsDirectory ? options.Directory.Substring( options.Directory.Replace( "\\", "/ " ).LastIndexOf( "/" ) + 1 ) 
        : 
        Path.GetFileNameWithoutExtension( options.File );

      if ( string.IsNullOrWhiteSpace( filename ) ) {
        if ( !string.IsNullOrWhiteSpace( exporter.DefaultFileNamePattern ) ) {
          filename = exporter.DefaultFileNamePattern;
        } else {
          filename = optionsFilename;
        }
      }
      return filename.Replace( "%assembly%", optionsFilename ).Replace( "%date%", DateTime.Now.ToString( "ddMMyyy" ) ) + "." + exporter.FileExtension;
    }
  }
}

