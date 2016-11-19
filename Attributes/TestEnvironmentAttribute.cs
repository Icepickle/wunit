using System;

namespace wUnit.Attributes
{
	/// <summary>
	/// Indicates that this class should be a part of the test class
	/// This class must have a public constructor, as it will be instantiated with the first constructor available at runtime
	/// Constructor arguments are resolved based on an InjectTypeParameterAttribute, an InjectValueParameterAttribute, a DefineImplementationAttribute or a DefaultValue
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class TestEnvironmentAttribute : AbstractActivatorAttribute
	{
		public string Environment { get; private set; }

		public TestEnvironmentAttribute (string environmentName, bool isEnabled = true)
		{
			IsEnabled = isEnabled;
			Environment = environmentName;
		}
	}
}

