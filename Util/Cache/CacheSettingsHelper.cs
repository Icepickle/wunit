using System;
using System.Linq;
using System.Collections.Generic;

namespace wUnit.Util.Cache
{
	/// <summary>
	/// Defines if the cache is enabled (by default true) and can be used to either set or read values from the cache
	/// </summary>
	public class CacheSettingsHelper<TKeyType, TCachedValueType> : ICacheSettings, ICacheHelper<TKeyType, TCachedValueType> {
		private object _dictLock = new object ();
		private readonly IDictionary<TKeyType, TCachedValueType> _cacheHolder = new Dictionary<TKeyType, TCachedValueType> ();

		public bool IsEnabled { get; set; }

		public TCachedValueType Get (TKeyType key)
		{
			lock (_dictLock) {
				return _cacheHolder [key];
			}
		}

		public bool TryGetValue (TKeyType key, out TCachedValueType value)
		{
			lock (_dictLock) {
				if (_cacheHolder.TryGetValue (key, out value)) {
					return true;
				}
			}
			return false;
		}

		public void Remove (TKeyType key)
		{
			lock (_dictLock) {
				_cacheHolder.Remove (key);
			}
		}

		public void Clear ()
		{
			lock (_dictLock) {
				var disposables = (from k in _cacheHolder
				                  where k.Value is IDisposable
					select k.Value).ToList();
				foreach (IDisposable disposable in disposables) {
					disposable.Dispose ();
				}
				_cacheHolder.Clear ();
			}
		}

		public void Set (TKeyType key, TCachedValueType value)
		{
			lock (_dictLock) {
				if (_cacheHolder.ContainsKey (key)) {
					throw new ArgumentException ("Key already exists!");
				}
				_cacheHolder.Add (key, value);
			}
		}

		public void SetOrReplace (TKeyType key, TCachedValueType value)
		{
			lock (_dictLock) {
				if (_cacheHolder.ContainsKey (key)) {
					_cacheHolder [key] = value;
					return;
				}
				_cacheHolder.Add (key, value);
			}
		}

		public TCachedValueType this[TKeyType key] {
			get {
				TCachedValueType result;
				if (TryGetValue (key, out result)) {
					return result;
				}
				return default(TCachedValueType);
			}
			set {
				SetOrReplace (key, value);
			}
		}

		private static Lazy<CacheSettingsHelper<TKeyType, TCachedValueType>> _current = new Lazy<CacheSettingsHelper<TKeyType, TCachedValueType>>(()=> new CacheSettingsHelper<TKeyType, TCachedValueType>(true));
		public static CacheSettingsHelper<TKeyType, TCachedValueType> Current {
			get {
				return _current.Value;
			}
		}

		public CacheSettingsHelper(bool isEnabled = true) {
			IsEnabled = isEnabled;
		}
	}
}

