using System;

namespace wUnit.Attributes
{
	/// <summary>
	/// Defines an implementation for an interface (or class) during the test cycle
	/// The implementations will be initialized as a singleton during runtime upon first need (as an argument for your methods)
	/// If the implementation implements IDisposable, the Dispose method will be called when the <see cref="TestEnvironmentAttribute"/> completes
	/// This attribute should be used together with the <see cref="TestEnvironmentAttribute"/>
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
	public class DefineImplementationAttribute : AbstractActivatorAttribute
	{
		public Type InterfaceType { get; private set; }
		public Type ImplementationType { get; private set; }

		public DefineImplementationAttribute (Type interfaceType, Type implementationType, bool isEnabled = true)
		{
			InterfaceType = interfaceType;
			ImplementationType = implementationType;
		}
	}
}

