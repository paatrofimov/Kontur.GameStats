using System;
using System.Linq;
using Kontur.GameStats.Database;
using Kontur.GameStats.Infrastructure;
using Kontur.GameStats.Infrastructure.Logging;
using Kontur.GameStats.Models.Json;
using Nancy;
using Nancy.ModelBinding;

namespace Kontur.GameStats.NancyModules
{
	public class MatchModule : NancyModule
	{
		public MatchModule()
		{
			Put["servers/{endpoint}/matches/{timestamp}"] = PutMatch;
			Get["servers/{endpoint}/matches/{timestamp}"] = GetMatch;
		}

		private object PutMatch(dynamic parameters)
		{
			var endpoint = (string) parameters.endpoint;
			var timestampParam = (string) parameters.timestamp;
			if (!timestampParam.TryParseFromIso8601(out var matchTimestamp))
				return HttpStatusCode.BadRequest;
			if (!DbQueries.ServerExists(endpoint))
			{
				ConsoleLogger.LogBadQuery(Request, HttpStatusCode.BadRequest, LogErrorMessages.AdvertisingError);
				return HttpStatusCode.BadRequest;
			}
			if (DbQueries.MatchExists(endpoint, matchTimestamp))
			{
				ConsoleLogger.LogBadQuery(Request, HttpStatusCode.MethodNotAllowed, LogErrorMessages.MatchesCollision);
				return HttpStatusCode.MethodNotAllowed;
			}
			var matchJson = this.Bind<MatchJsonModel>();
			var scoreboardHasCollision = matchJson.Scoreboard.Distinct().Count() < matchJson.Scoreboard.Count;
			if (scoreboardHasCollision)
			{
				ConsoleLogger.LogBadQuery(Request, HttpStatusCode.MethodNotAllowed, LogErrorMessages.ScoreboardCollision);
				return HttpStatusCode.MethodNotAllowed;
			}
			DbQueries.UpdateDatabaseWithMatch(matchTimestamp, endpoint, matchJson);
			return HttpStatusCode.OK;
		}

		private object GetMatch(dynamic parameters)
		{
			var endpoint = (string) parameters.endpoint;
			DateTime timestamp;
			if (!((string) parameters.timestamp).TryParseFromIso8601(out timestamp))
				return HttpStatusCode.BadRequest;
			var matchJson = DbQueries.TrySelectMatch(endpoint, timestamp);
			if (matchJson == null)
			{
				ConsoleLogger.LogBadQuery(Request, HttpStatusCode.NotFound, LogErrorMessages.MissingMatch);
				return HttpStatusCode.NotFound;
			}
			return matchJson;
		}
	}
}