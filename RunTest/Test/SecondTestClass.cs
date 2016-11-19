using System;
using wUnit.Attributes;
using wUnit.Util;

namespace RunTest.Test
{
	[TestEnvironment ("Second class")]
	[DefineImplementation (typeof(ILogger), typeof(ConsoleLogger))]
	public class ParameterTypeTest
	{
		[RunTest]
		public void ParametersAreOfTheCorrectType (ILogger logger, [InjectTypeParameter (typeof(InternalTestClass))]object internalClass)
		{
			Assert.IsNotNull (internalClass);
			Assert.IsOfType (internalClass, typeof(InternalTestClass));
			Assert.IsEqual (((InternalTestClass)internalClass).Logger, logger);
		}
	}
}

