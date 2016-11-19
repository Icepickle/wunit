using System;
using System.Collections.Generic;
using wUnit.Util;

namespace wUnit.Model
{
	/// <summary>
	/// Indicates that this class has some Types linked to a RunTime type
	/// </summary>
	public interface IDefineTypes {
		IContextRunnerConfiguration ContextRunnerConfiguration { get; }
		IDictionary<Type, Type> DefinedTypes { get; }
	}
}

