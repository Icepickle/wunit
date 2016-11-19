using System;
using wUnit;
using System.Threading;
using wUnit.Util;

namespace RunTest
{
	public enum LogLevel : int {
		DEBUG = 0,
		ERROR = 1,
		INFO = 2,
		LOG = 3,
		SUCCESS = -1
	}

	public class ConsoleLogger :  AbstractSynchronizationClass, ILogger
	{
		public LogLevel MaxLogLevel { get; set; }

		protected void Log(LogLevel level, string message) {
			if (level > MaxLogLevel) {
				return;
			}
			if (level == LogLevel.ERROR) {
				FireAndWait ((obj) => {
					System.Diagnostics.Debug.WriteLine(string.Format("Current thread: #{0}", System.Threading.Thread.CurrentThread.ManagedThreadId));
					ConsoleColor color = Console.ForegroundColor;
					Console.ForegroundColor = ConsoleColor.Red;
					Console.Error.WriteLine("{0}: {1}", level, message);
					Console.ForegroundColor = color;
				});
			} else {
				FireAndWait ((obj) => {
					System.Diagnostics.Debug.WriteLine(string.Format("Current thread: #{0}", System.Threading.Thread.CurrentThread.ManagedThreadId));
					ConsoleColor color = Console.ForegroundColor;
					if (level == LogLevel.DEBUG) {
						Console.ForegroundColor = ConsoleColor.Yellow;
					} else if (level == LogLevel.LOG) {
						Console.ForegroundColor = ConsoleColor.DarkGray;
					} else if (level == LogLevel.INFO) {
						Console.ForegroundColor = ConsoleColor.White;
					} else {
						Console.ForegroundColor = ConsoleColor.Green;
					}
					Console.WriteLine("{0}: {1}", level, message);
					Console.ForegroundColor = color;
				});
			}
		}

		protected string ReadException(Exception ex) {
			if (ex == null) {
				return null;
			}
			string innerException = ReadException (ex.InnerException);
			if (string.IsNullOrEmpty (innerException)) {
				return ex.Message + Environment.NewLine + ex.StackTrace;
			} 
			return ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + innerException;
		}

		public void Log (string format, params object[] formatValues)
		{
			Log (LogLevel.LOG, string.Format (format, formatValues));
		}

		public void Info (string format, params object[] formatValues)
		{
			Log (LogLevel.INFO, string.Format (format, formatValues));
		}

		public void Debug (string format, params object[] formatValues)
		{
			Log (LogLevel.DEBUG, string.Format (format, formatValues));
		}

		public void Error (Exception ex, string format, params object[] formatValues)
		{
			Log (LogLevel.ERROR, string.Format (format, formatValues) + Environment.NewLine + ReadException(ex));
		}

		public void Success(string format, params object[] formatValues) {
			Log (LogLevel.SUCCESS, string.Format (format, formatValues));
		}

		public ConsoleLogger (LogLevel maxLogLevel = LogLevel.LOG)
		{
			MaxLogLevel = maxLogLevel;
		}
	}
}

