using System;
using wUnit.Util;

namespace RunTest.Test
{
	internal class InternalTestClass : IDisposable
	{
		public ILogger Logger { get; set; }

		public void Dispose ()
		{
			Logger = null;
		}

		internal InternalTestClass (ILogger logger)
		{
			Logger = logger;
		}
	}
}

