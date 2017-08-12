using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;

namespace Kontur.GameStats.Infrastructure.Logging
{
	public static class ConsoleLogger // todo log4net
	{
		public static List<string> Events { get; } = new List<string>();

		public static string LogBadQuery(Request request, HttpStatusCode responseCode, string msg = "")
		{
			var logged =
				$"Bad query: {request.Method} {request.Url} \r\n" +
				$"\t responded with http code '{responseCode}' \r\n" +
				$"Message: {msg} \r\n\r\n";
			Events.Add(logged);
			Console.WriteLine(logged);
			return logged;
		}

		public new static string ToString() => Events.Aggregate((s1, s2) => s1 + s2);
	}
}