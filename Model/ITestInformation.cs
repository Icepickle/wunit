using System;
using System.ComponentModel;
using System.Reflection;
using System.Collections.Generic;

namespace wUnit.Model
{
	/// <summary>
	/// Describes a TestMethod, total runtime and if it has succeeded, failed or timedout
	/// </summary>
	public interface ITestInformation : INotifyPropertyChanged {
		MethodInfo TargetMethod { get; }
		Attribute Attribute { get; set; }
		IList<ITestRunInformation> Runs { get; }
		bool Called { get; set; }
	}
}

