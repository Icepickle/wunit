using System;
using wUnit.Attributes;
using wUnit.Util;

namespace RunTest.Test {
  [TestEnvironment( "First test" )]
  [DefineImplementation( typeof(ILogger), typeof(ConsoleLogger) )]
  [DefineImplementation( typeof(IContextRunner), typeof(TestSuiteContextRunner) )]
  public class TestFile {
    private string _aString;

    [InitializeTest]
    public void SetupTest( ILogger logger ) {
      Assert.IsOfType<ConsoleLogger>( logger );
      _aString = "This is a text";
    }

    [DestroyTest]
    public void DestroyTest( ILogger logger ) {
      _aString = null;
    }

    [RunTest]
    public void RunTest( ILogger logger, [InjectValueParameter( 5 )]int value, IContextRunner runner ) {
      Assert.IsNotNull( logger );
      Assert.IsNotEqual( _aString.Length, 0 );
      Assert.IsEqual( value, 5 );
      Assert.IsNotNull( runner );
    }

    [RunTest]
    public void RunTest2() {
    }

    [PreTest]
    public void WillRunBeforeEveryTest() {
    }

    [PostTest]
    public void WillRunAfterEveryTest() {
    }
  }
}

