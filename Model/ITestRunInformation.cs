using System;
using System.ComponentModel;

namespace wUnit.Model
{
	public interface ITestRunInformation : INotifyPropertyChanged {
		bool Called { get; set; }
		DateTime Start { get; set; }
		DateTime End { get; set; }
		bool Completed { get; set; }
		bool Running { get; set; }
		ResultState State { get; set; }
	}
}

