using System;
using System.IO;

namespace wUnit.Model
{
	/// <summary>
	/// Describes an error inside the test environment
	/// The error can occure during initialization of the TestEnvironment, the reading of the Assemblies or during an Initialize/Run/Destroy method
	/// </summary>
	public class TestErrorOccuredEventArgs : ErrorEventArgs
	{
		public ITestConfiguration TestConfiguration { get; private set; }

		public ITestInformation TestInformation { get; private set; }

		public Exception Exception { get; private set; }

		public ResultState ResultState { get; private set; }

		public TestErrorOccuredEventArgs (ITestConfiguration configuration, ITestInformation information, Exception ex = null, ResultState state = ResultState.FAILED)
			: base(ex)
		{
			TestConfiguration = configuration;
			TestInformation = information;
			Exception = ex;
			ResultState = state;
		}
	}
}

