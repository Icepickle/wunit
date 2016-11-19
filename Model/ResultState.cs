using System;

namespace wUnit.Model
{
	/// <summary>
	/// The current state of a test
	/// </summary>
	public enum ResultState {
		/// <summary>
		/// Test didn't run yet
		/// </summary>
		INACTIVE,
		/// <summary>
		/// Test is running
		/// </summary>
		RUNNING,
		/// <summary>
		/// Test succeeded and ran within timeconstraints (if any were defined)
		/// </summary>
		SUCCEEDED,
		/// <summary>
		/// Test method timed out
		/// </summary>
		TIMEDOUT,
		/// <summary>
		/// Test method ran into an exception
		/// </summary>
		FAILED
	}
}

