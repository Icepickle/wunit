using System;

namespace wUnit.Attributes
{
	/// <summary>
	/// Defines the max time a test can run before it is forcefully ended and returns an error with state ResultState.TIMEOUT
	/// It can be used on either a class or a method
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class MaxTimeAttribute : AbstractActivatorAttribute
	{
		/// <summary>
		/// Milliseconds
		/// </summary>
		/// <value>The time out after value in Milliseconds</value>
		public int TimeOutAfter { get; private set; }

		public MaxTimeAttribute (int timeOutAfter, bool isEnabled = true)
		{
			TimeOutAfter = timeOutAfter;
			IsEnabled = isEnabled;
		}
	}
}

