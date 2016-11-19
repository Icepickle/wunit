using System;

namespace wUnit.Attributes
{
	/// <summary>
	/// Indicates that this method should be called after the actual tests. It can be used to clean up resources that where defined inside the class.
	/// Optionally your Test class can also implement the IDisposable interface and then the Dispose method will be called
	/// A test class can implement more than 1 method with the DestroyTestAttribute. Running order is decided at runtime
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class DestroyTestAttribute : AbstractActivatorAttribute
	{
		public string Description { get; set; }

		public DestroyTestAttribute (string description = "", bool isEnabled = true)
		{
			this.IsEnabled = isEnabled;
			this.Description = description;
		}
	}
}

