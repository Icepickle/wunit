using System;

namespace wUnit.Attributes
{
	/// <summary>
	/// Injects a value at runtime when the method is called. The value will be converted using Convert.ChangeType at runtime
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter)]
	public class InjectValueParameterAttribute : AbstractActivatorAttribute
	{
		public object Value { get; set; }

		public InjectValueParameterAttribute (object value, bool isEnabled = true)
		{
			Value = value;
			IsEnabled = isEnabled;
		}
	}
}

