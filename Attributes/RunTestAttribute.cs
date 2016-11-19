using System;

namespace wUnit.Attributes
{
	/// <summary>
	/// Indicates that this method has to be called when running the test class
	/// The method must be public, but can contain parameters
	/// Parameters are resolved with either a DefineImplementationAttribute, an InjectTypeParameterAttribute on parameter level, an InjectValueParameter attribute or a DefaultValue
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class RunTestAttribute : AbstractSequencedAttribute
	{
		public string Description { get; private set; }
		public uint MinimumRunCount { get; private set; }
		public uint MaximumRetryCount { get; private set; }

		public RunTestAttribute (string description = "", bool isEnabled = true, uint minimumRunCount = 1, uint maximumRetryCount = 0, int sequenceNr = 0)
		{
			this.IsEnabled = isEnabled;
			this.MinimumRunCount = minimumRunCount;
			this.MaximumRetryCount = maximumRetryCount;
			this.Description = description;
			this.SequenceNumber = sequenceNr;
		}
	}
}

