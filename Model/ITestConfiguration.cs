using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace wUnit.Model
{
	/// <summary>
	/// Describes a TestEnvironment by Type and found tests
	/// Can indicate if it is Completed and how many Failed tests methods have happened
	/// </summary>
	public interface ITestConfiguration : INotifyPropertyChanged, IDefineTypes
	{
		Type Type { get; }
		IList<ITestInformation> Tests { get; }
		bool Completed { get; }
		int Failed { get; }
	}
}

