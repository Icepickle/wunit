using System;
using wUnit.Attributes;
using wUnit.Export;
using RunTest.Export;
using wUnit.Util;
using System.IO;

namespace RunTest {
  [TestEnvironment( "Test if filename helper gives correct results" )]
  [DefineImplementation( typeof(IConfigurationResultExporter), typeof(ConsoleExporter) )]
  [DefineImplementation( typeof(ProgramOptions), typeof(ProgramOptions) )]
  public class TestExportFileNameHelper {
    [RunTest]
    public void TestWithNoFileName( IConfigurationResultExporter exporter, ProgramOptions options ) {
      string nameWithEmptyString = ExportFilenameHelper.GetFilenameForExportConfiguration( exporter, string.Empty, options );
      string nameWithNullString = ExportFilenameHelper.GetFilenameForExportConfiguration( exporter, null, options );
      string patternName = ExportFilenameHelper.GetFilenameForExportConfiguration( exporter, exporter.DefaultFileNamePattern, options );
      Assert.IsEqual( nameWithEmptyString, nameWithNullString );
      Assert.IsEqual( nameWithNullString, patternName );
    }

    [RunTest]
    public void TestWithFilename( IConfigurationResultExporter exporter, ProgramOptions options ) {
      string runTestFilename = ExportFilenameHelper.GetFilenameForExportConfiguration( exporter, "runtest", options );
      Assert.IsEqual( runTestFilename, "runtest.xml" );
    }

    [RunTest]
    public void TestWithDateVariable( IConfigurationResultExporter exporter, ProgramOptions options ) {
      string runTestFileName = ExportFilenameHelper.GetFilenameForExportConfiguration( exporter, "%date%-runtest", options );
      Assert.IsEqual( runTestFileName, DateTime.Now.ToString( "ddMMyyy" ) + "-runtest.xml" );
    }

    [RunTest]
    public void TestWithAssemblyVariable( IConfigurationResultExporter exporter, ProgramOptions options ) {
      string current = Environment.CurrentDirectory;
      current = current.Substring( current.Replace( "\\", "/" ).LastIndexOf( "/" ) + 1 );
      string expected = DateTime.Now.ToString( "ddMMyyy" ) + "-%state%-" + current + ".xml";
      string runTestFileName = ExportFilenameHelper.GetFilenameForExportConfiguration( exporter, null, options );
      Assert.IsEqual( runTestFileName, expected );
    }
  }
}

