using System;

namespace wUnit.Attributes
{
	/// <summary>
	/// Indicates that this method should be called before the actual tests. It can be used to initialize internal resources that will be used during the tests
	/// A test class can have more than one InitializeTestAttribute. Running order is decided at runtime
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class InitializeTestAttribute : AbstractActivatorAttribute
	{
		public string Description { get; set; }

		public InitializeTestAttribute (string description = "", bool isEnabled = true)
		{
			this.IsEnabled = isEnabled;
			this.Description = description;
		}
	}
}

