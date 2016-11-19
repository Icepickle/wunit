using System.Collections.Generic;
using System.Reflection;
using wUnit.Model;

namespace wUnit.Util
{
	public interface IContextRunnerConfiguration
	{
		System.AppDomain ContextAppDomain { get; }
		Assembly[] TargetAssemblies { get; }
		IList<ITestConfiguration> Configurations { get; }
	}
}

