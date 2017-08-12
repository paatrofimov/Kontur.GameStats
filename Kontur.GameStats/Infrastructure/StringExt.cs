using System;
using System.Globalization;

namespace Kontur.GameStats.Infrastructure
{
	public static class StringExt
	{
		public static bool TryParseFromIso8601(this string iso, out DateTime result)
		{
			return DateTime.TryParseExact(iso, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture,
				DateTimeStyles.RoundtripKind, out result);
		}
	}
}