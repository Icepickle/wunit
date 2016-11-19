using System;
using wUnit.Model;
using System.Reflection;

namespace wUnit.Util.Factory
{
	/// <summary>
	/// Describes what an EntityFactory should implement to be compatible with the test environment
	/// </summary>
	public interface IEntityFactory {
		object ConstructType (Type t, IDefineTypes typeDefinitions);
		T ConstructType<T> (IDefineTypes typeDefinitions);
		object[] FillParamInfo(ParameterInfo[] parameters, IDefineTypes typeDefinitions);
	}
}

