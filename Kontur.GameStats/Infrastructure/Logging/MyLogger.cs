using log4net;
using Nancy;

namespace Kontur.GameStats.Infrastructure.Logging
{
	public static class MyLogger
	{
		private static ILog instance = LogManager.GetLogger("EpicLoggerName");

		public static void LogBadQuery(Request request, HttpStatusCode responseCode, string msg = "")
		{
			instance.Error(
				$"Bad query: {request.Method} {request.Url}. Responded with http code '{responseCode}'. Message: {msg}");
		}
	}
}