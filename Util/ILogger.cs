using System;

namespace wUnit.Util
{
	public interface ILogger: IDisposable
	{
		void Log(string format, params object[] formatValues);
		void Info(string format, params object[] formatValues);
		void Debug(string format, params object[] formatValues);
		void Error(Exception ex, string format, params object[] formatValues);
		void Success (string format, params object[] formatValues);
	}
}

