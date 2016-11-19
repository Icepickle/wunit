using System;

namespace wUnit.Util.Cache
{
	/// <summary>
	/// Describes wether the cache is enabled or not
	/// </summary>
	public interface ICacheSettings {
		bool IsEnabled { get; set; }
	}
}

