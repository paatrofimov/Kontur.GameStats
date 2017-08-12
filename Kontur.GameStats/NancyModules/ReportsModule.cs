using Kontur.GameStats.Database;
using Nancy;

namespace Kontur.GameStats.NancyModules
{
	public class ReportsModule : NancyModule
	{
		public ReportsModule()
		{
			Get["/reports/recent-matches/{count:min(1)}"] = GetRecentMatches;
			Get["/reports/recent-matches"] = GetRecentMatches;
			Get["/reports/popular-servers/{count:min(1)}"] = GetPopularServers;
			Get["/reports/popular-servers"] = GetPopularServers;
			Get["/reports/best-players/{count:min(1)}"] = GetBestPlayers;
			Get["/reports/best-players"] = GetBestPlayers;
		}

		private const int defaultCount = 5;

		private object GetRecentMatches(dynamic parameters)
		{
			if (!int.TryParse(parameters.count, out int count))
				count = defaultCount;
			return DbQueries.SelectRecentMatches(count);
		}

		private object GetPopularServers(dynamic parameters)
		{
			if (!int.TryParse(parameters.count, out int count))
				count = defaultCount;
			return DbQueries.SelectPopularServers(count);
		}

		private object GetBestPlayers(dynamic parameters)
		{
			if (!int.TryParse(parameters.count, out int count))
				count = defaultCount;
			return DbQueries.SelectBestPlayers(count);
		}
	}
}