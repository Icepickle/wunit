using System;

namespace wUnit.Attributes
{
	public class PostTestAttribute : AbstractActivatorAttribute
	{
		public PostTestAttribute (bool isEnabled = true) {
			IsEnabled = isEnabled;
		}
	}
}

