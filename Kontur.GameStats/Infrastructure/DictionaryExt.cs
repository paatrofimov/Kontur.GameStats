using System.Collections.Generic;

namespace Kontur.GameStats.Infrastructure
{
	public static class DictionaryExt
	{
		public static void AddOrIncrement<T>(this Dictionary<T, int> dict, T key)
		{
			if (dict.ContainsKey(key))
				dict[key]++;
			else
			{
				dict[key] = 1;
			}
		}
	}
}