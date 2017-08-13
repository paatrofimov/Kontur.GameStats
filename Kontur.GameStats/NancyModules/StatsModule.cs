using Kontur.GameStats.Database;
using Kontur.GameStats.Infrastructure.Logging;
using Nancy;

namespace Kontur.GameStats.NancyModules
{
	public class StatsModule : NancyModule
	{
		public StatsModule()
		{
			Get["/servers/{endpoint}/stats"] = GetServerStats;
			Get["/players/{name}/stats"] = GetPlayerStats;
		}

		private object GetServerStats(dynamic parameters)
		{
			var endpoint = (string) parameters.endpoint;
			var serverStats = DbQueries.TrySelectServerStats(endpoint);
			if (serverStats == null)
			{
				MyLogger.LogBadQuery(Request, HttpStatusCode.NotFound,
					LogErrorMessages.MissingServerstats);
				return HttpStatusCode.NotFound;
			}
			return serverStats;
		}

		private object GetPlayerStats(dynamic parameters)
		{
			var encodedPlayer = (string) parameters.name;
			var playerJson = DbQueries.TrySelectPlayer(encodedPlayer);
			if (playerJson == null)
			{
				MyLogger.LogBadQuery(Request, HttpStatusCode.NotFound,
					LogErrorMessages.MissingPlayer);
				return HttpStatusCode.NotFound;
			}
			return playerJson;
		}
	}
}