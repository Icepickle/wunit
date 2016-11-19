using System;

namespace wUnit.Attributes
{
	public class PreTestAttribute : AbstractActivatorAttribute
	{
		public PreTestAttribute (bool isEnabled = true) {
			IsEnabled = isEnabled;
		}
	}
}

