using System;

namespace wUnit.Util.Cache
{
	/// <summary>
	/// Defines a generic cache helper
	/// </summary>
	public interface ICacheHelper<TKeyType, TCachedValueType> {
		TCachedValueType Get (TKeyType key);
		bool TryGetValue (TKeyType key, out TCachedValueType value);
		void Remove(TKeyType key);
		void Clear ();
		void Set (TKeyType key, TCachedValueType value);
		void SetOrReplace (TKeyType key, TCachedValueType value);
	}
}

