using System;

namespace wUnit.Attributes
{
	/// <summary>
	/// Indicates that an arguments should be initialized with a certain Type. At runtime, the IContextRunner must initialize this type.
	/// After the method ran through, the Type will be disposed in case it implements the IDisposable interface.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter)]
	public class InjectTypeParameterAttribute : AbstractActivatorAttribute
	{
		public Type Type { get; private set; }

		public InjectTypeParameterAttribute (Type type, bool isEnabled = true)
		{
			Type = type;
			IsEnabled = isEnabled;
		}
	}
}

