using System;
using System.Globalization;

namespace Kontur.GameStats.Infrastructure
{
	public static class DateTimeExt
	{
		public static string ToIso8601(this DateTime date)
		{
			return date.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
		}
	}
}