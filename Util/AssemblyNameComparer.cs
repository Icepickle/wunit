using System;
using System.Collections.Generic;
using System.Reflection;

namespace wUnit.Util
{
	public class AssemblyNameComparer : IEqualityComparer<AssemblyName>
	{
		#region IEqualityComparer<AssemblyName> Members
		public bool Equals (AssemblyName x, AssemblyName y)
		{
			return string.Equals (x.FullName, y.FullName);
		}

		public int GetHashCode (AssemblyName obj)
		{
			return obj.FullName.GetHashCode ();
		}
		#endregion
	}

}

