using System;
using System.ComponentModel;
using wUnit.Util;

namespace wUnit.Model
{
	public class TestRunInformation : AbstractSynchronizationClass, ITestRunInformation
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private bool _called;
		private DateTime _start;
		private DateTime _end;
		private bool _completed;
		private bool _running;
		private ResultState _state;

		public bool Called {
			get {
				return _called;
			}
			set {
				if (_called == value) {
					return;
				}
				_called = value;
				RaisePropertyChanged ("Called");
			}
		}
		public DateTime Start {
			get {
				return _start;
			}
			set {
				if (_start == value) {
					return;
				}
				_start = value;
				RaisePropertyChanged ("Start");
			}
		}
		public DateTime End {
			get {
				return _end;
			}
			set {
				if (_end == value) {
					return;
				}
				_end = value;
				RaisePropertyChanged ("End");
			}
		}
		public bool Completed {
			get {
				return _completed;
			}
			set {
				if (_completed == value) {
					return;
				}
				_completed = value;
				RaisePropertyChanged ("Completed");
			}
		}
		public bool Running {
			get {
				return _running;
			}
			set {
				if (_running == value) {
					return;
				}
				_running = value;
				RaisePropertyChanged ("Running");
			}
		}

		public ResultState State {
			get {
				return _state;
			}
			set {
				if (_state == value) {
					return;
				}
				_state = value;
				RaisePropertyChanged ("State");
			}
		}

		protected virtual void RaisePropertyChanged(string propertyName) {
			var local = PropertyChanged;
			if (local != null) {
				Fire ((s) => {
					local.Invoke (this, new PropertyChangedEventArgs (propertyName));
				});
			}
		}

		public TestRunInformation ()
		{
		}
	}
}

