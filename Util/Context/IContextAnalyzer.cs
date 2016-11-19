using System;
using wUnit.Model;
using wUnit.Util;
using System.IO;

namespace wUnit.Util.Context {
	public interface IContextAnalyzer {
		event EventHandler<ErrorEventArgs> ErrorOccured;
		void CreateConfiguration(IContextRunnerConfiguration configuration, Action callback);
	}
}

