using System;
using System.Collections.Generic;
using System.Linq;
using Kontur.GameStats.Models.Entities;
using Kontur.GameStats.Models.Json;
using Nancy.Helpers;
using Newtonsoft.Json;

namespace Kontur.GameStats.Database
{
	public static class DbQueries
	{
		public static ServerStatsJsonModel TrySelectServerStats(string endpoint)
		{
			using (var ctx = new MyDbContext())
			{
				var entity = ctx.Servers.Find(endpoint);
				return entity == null ? null : new ServerStatsJsonModel(entity);
			}
		}

		public static PlayerJsonModel TrySelectPlayer(string encodedName)
		{
			var decodedName = HttpUtility.UrlDecode(encodedName);
			using (var ctx = new MyDbContext())
			{
				var entity = ctx.Players.Find(decodedName);
				if (entity == null)
					return null;
				FindLatestGameDate(ctx, out var latestGameDate);
				return new PlayerJsonModel(entity, latestGameDate);
			}
		}

		public static ServerInfoJsonModel TrySelectServerInfo(string endpoint)
		{
			using (var ctx = new MyDbContext())
			{
				var entity = ctx.Servers.Find(endpoint);
				return entity == null ? null : new ServerInfoJsonModel(entity);
			}
		}

		public static void UpsertServerInfo(string endpoint, ServerInfoJsonModel serverInfoJson)
		{
			using (var ctx = new MyDbContext())
			{
				var entity = new ServerEntityModel
				{
					Endpoint = endpoint,
					Server = serverInfoJson.Name,
					GameModes = JsonConvert.SerializeObject(serverInfoJson.GameModes)
				};
				var oldServer = ctx.Servers.Find(endpoint);
				if (oldServer != null)
				{
					ctx.Servers.Remove(oldServer);
				}
				ctx.Servers.Add(entity);
				ctx.SaveChanges();
			}
		}

		public static AllServerInfoJsonModel[] SelectAllServerInfos()
		{
			using (var ctx = new MyDbContext())
			{
				return ctx.Servers
					.ToArray()
					.Select(entity => new AllServerInfoJsonModel(entity))
					.OrderBy(json => json.Endpoint)
					.ToArray();
			}
		}

		public static bool ServerExists(string endpoint)
		{
			using (var ctx = new MyDbContext())
			{
				return ctx.Servers.Find(endpoint) != null;
			}
		}

		public static bool MatchExists(string endpoint, DateTime timestamp)
		{
			using (var ctx = new MyDbContext())
			{
				return ctx.Matches.Find(endpoint, timestamp.Ticks) != null;
			}
		}

		public static void UpdateDatabaseWithMatch(DateTime matchTimestamp, string endpoint, MatchJsonModel matchJson)
		{
			using (var ctx = new MyDbContext())
			{
				InsertMatch(matchJson, matchTimestamp, endpoint, ctx);
				UpsertPlayersOfMatch(matchJson, matchTimestamp, endpoint, ctx);
				UpdateServerEntityWithMatch(matchJson, matchTimestamp.Date, endpoint, ctx);
				ctx.SaveChanges();
			}
		}

		private static void InsertMatch(MatchJsonModel matchJson, DateTime matchTimestamp, string endpoint, MyDbContext ctx)
		{
			ctx.Matches.Add(
				new MatchEntityModel
				{
					Endpoint = endpoint,
					FragLimit = matchJson.FragLimit,
					Map = matchJson.Map,
					GameMode = matchJson.GameMode,
					TimeElapsed = matchJson.TimeElapsed,
					TimeLimit = matchJson.TimeLimit,
					TimestampTicks = matchTimestamp.Ticks,
					Scoreboard = JsonConvert.SerializeObject(matchJson.Scoreboard)
				});
		}

		private static void UpsertPlayersOfMatch(MatchJsonModel matchJson, DateTime matchTimestamp, string endpoint,
			MyDbContext ctx)
		{
			foreach (var record in matchJson.Scoreboard)
			{
				var playerEntity = ctx.Players.FirstOrDefault(entity => entity.Player == record.Player);
				if (playerEntity == null)
				{
					playerEntity = new PlayerEntityModel {Player = record.Player,};
					ctx.Players.Add(playerEntity);
				}
				playerEntity.Update(matchJson, matchTimestamp, endpoint);
			}
		}

		private static void UpdateServerEntityWithMatch(MatchJsonModel matchJson, DateTime matchDate, string endpoint,
			MyDbContext ctx)
		{
			var serverEntity = ctx.Servers.Single(entity => entity.Endpoint.Equals(endpoint));
			var latestGameDate = matchDate;
			if (FindLatestGameDate(ctx, out DateTime latest) && latest.Ticks > latestGameDate.Ticks)
			{
				latestGameDate = latest;
			}
			serverEntity.Update(matchJson, matchDate, latestGameDate);
		}

		private static bool FindLatestGameDate(MyDbContext ctx, out DateTime result)
		{
			result = DateTime.MinValue;
			var serversMatchesPerDateSerialized = ctx.Servers
				.Where(entity => entity.MatchesPerDate != String.Empty)
				.Select(entity => entity.MatchesPerDate)
				.ToList();
			if (!serversMatchesPerDateSerialized.Any())
				return false;
			foreach (var matchesPerDateSerialized in serversMatchesPerDateSerialized)
			{
				var matchesPerDate = JsonConvert.DeserializeObject<Dictionary<DateTime, int>>(matchesPerDateSerialized);
				var latestServerMatchDate = matchesPerDate.Keys.Max().Date;
				if (result.Ticks < latestServerMatchDate.Ticks)
					result = latestServerMatchDate;
			}
			return true;
		}

		public static MatchJsonModel TrySelectMatch(string endpoint, DateTime timestamp)
		{
			using (var ctx = new MyDbContext())
			{
				var matchEntity = ctx.Matches.Find(endpoint, timestamp.Ticks);
				return matchEntity == null ? null : new MatchJsonModel(matchEntity);
			}
		}

		public static RecentMatchJsonModel[] SelectRecentMatches(int count)
		{
			using (var ctx = new MyDbContext())
			{
				return ctx.Matches
					.OrderByDescending(entity => entity.TimestampTicks)
					.Take(count)
					.ToArray()
					.Select(entity => new RecentMatchJsonModel(entity))
					.ToArray();
			}
		}

		public static PopularServerJsonModel[] SelectPopularServers(int count)
		{
			using (var ctx = new MyDbContext())
			{
				return ctx.Servers
					.OrderByDescending(entity => entity.AvgMatchesPerDay)
					.Take(count)
					.ToArray()
					.Select(entity => new PopularServerJsonModel(entity))
					.ToArray();
			}
		}

		public static BestPlayerJsonModel[] SelectBestPlayers(int count)
		{
			using (var ctx = new MyDbContext())
			{
				return ctx.Players
					.Where(entity => entity.TotalMatchesPlayed >= 10)
					.ToArray()
					.Select(entity => new BestPlayerJsonModel(entity))
					.OrderByDescending(json => json.KillToDeathRatio)
					.Take(count)
					.ToArray();
			}
		}
	}
}