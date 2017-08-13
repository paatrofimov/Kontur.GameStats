using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Kontur.GameStats.Database;
using Kontur.GameStats.Models.Json;
using Kontur.GameStats.Tests.TestHelpers;
using Nancy.Testing;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Kontur.GameStats.Tests
{
	[TestFixture]
	public class Statserver_Should
	{
		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			MyDbContext.IsForUnitTests = true;
			PutAllServerInfos();
			PutAllMatches();
		}

		private static void PutAllServerInfos()
		{
			for (var i = 0; i < TestData.Endpoints.Count; i++)
			{
				MakeRequest.PutServerInfo(TestData.Endpoints[i], TestData.ServerInfoJsons[i]);
			}
		}

		private void PutAllMatches()
		{
			foreach (var match in TestData.MatchEntities)
			{
				MakeRequest.PutMatch(match);
			}
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			DbCleaner.ClearTables();
		}

		[Test]
		public void Get_ServerInfo()
		{
			var response = MakeRequest.GetServerInfo(TestData.Endpoints[0]);

			var info = JsonConvert.DeserializeObject<ServerInfoJsonModel>(response.Body.AsString());
			info.Should()
				.Be(new ServerInfoJsonModel
				{
					GameModes = TestData.ServerInfoJsons[0].GameModes,
					Name = TestData.ServerInfoJsons[0].Name
				});
		}

		[Test]
		public void Get_All_ServerInfos()
		{
			var response = MakeRequest.GetAllServerInfos();

			var allInfos = JsonConvert.DeserializeObject<List<AllServerInfoJsonModel>>(response.Body.AsString());
			TestData.Endpoints.Should().OnlyContain(e => allInfos.Select(each => each.Endpoint).Contains(e));
			var infos = allInfos.Select(each => JsonConvert.DeserializeObject<ServerInfoJsonModel>(each.Info));
			TestData.ServerInfoJsons.Should().OnlyContain(i => infos.Contains(i));
		}

		[Test]
		public void Get_Match()
		{
			var response = MakeRequest.GetMatch(TestData.MatchEntities[0].Endpoint,
				new DateTime(TestData.MatchEntities[0].TimestampTicks));

			var match = JsonConvert.DeserializeObject<MatchJsonModel>(response.Body.AsString());
			match.Should().Be(new MatchJsonModel(TestData.MatchEntities[0]));
		}

		private double gameTotalDays = new TimeSpan(TestData.Timestamps[1].Date.Ticks - TestData.Timestamps[2].Date.Ticks)
			                               .TotalDays + 1;

		[Test]
		public void Get_ServerStats()
		{
			var response = MakeRequest.GetServerStats(TestData.Endpoints[0]);

			var stats = JsonConvert.DeserializeObject<ServerStatsJsonModel>(response.Body.AsString());
			stats.Should()
				.Be(new ServerStatsJsonModel
				{
					TotalMatchesPlayed = 4,
					MaximumMatchesPerDay = 2,
					MaximumPopulation = 3,
					AverageMatchesPerDay = 4 / gameTotalDays,
					AveragePopulation = 3,
					Top5Maps = TestData.Maps.Take(3).ToList(),
					Top5GameModes = TestData.Modes.Take(2).ToList()
				});
		}

		[Test]
		public void Get_Player()
		{
			var response = MakeRequest.GetPlayer(TestData.PlayersLeaderboard[0]);

			var player = JsonConvert.DeserializeObject<PlayerJsonModel>(response.Body.AsString());
			player.Should()
				.Be(new PlayerJsonModel
				{
					TotalMatchesPlayed = 10,
					TotalMatchesWon = 10,
					FavoriteServer = TestData.Endpoints[0],
					UniqueServers = 3,
					FavoriteGameMode = TestData.Modes[0],
					AverageScoreboardPercent = 100,
					MaximumMatchesPerDay = 6,
					AverageMatchesPerDay = 10 / gameTotalDays,
					LastMatch = TestData.Timestamps[1],
					KillToDeathRatio = 1.0 * TestData.Records[0].Kills / TestData.Records[0].Deaths
				});
		}

		[Test]
		[TestCase(-1)]
		[TestCase(2)]
		[TestCase(6)]
		[TestCase(100)]
		public void Get_Reports_RecentMatches(int count)
		{
			var response = count == -1
				? MakeRequest.GetRecentMatches()
				: MakeRequest.GetRecentMatches(count);
			count = ValidateCountParameter(count, TestData.MatchEntities.Count);

			var recentMatches = JsonConvert.DeserializeObject<RecentMatchJsonModel[]>(response.Body.AsString());
			recentMatches.SequenceEqual(
					TestData.MatchEntities
						.OrderByDescending(entity => entity.TimestampTicks)
						.Select(entity => new RecentMatchJsonModel(entity))
						.Take(count))
				.Should()
				.BeTrue();
		}

		private static int ValidateCountParameter(int count, int collectionLength)
		{
			const int minVal = 5;
			const int maxVal = 50;
			if (count == -1)
				count = minVal;
			return Math.Min(Math.Min(count, maxVal), collectionLength);
		}

		[Test]
		[TestCase(-1)]
		[TestCase(2)]
		[TestCase(6)]
		[TestCase(100)]
		public void Get_Reports_PopularServers(int count)
		{
			var response = count == -1
				? MakeRequest.GetPopularServers()
				: MakeRequest.GetPopularServers(count);
			count = ValidateCountParameter(count, TestData.PopularServers.Count);

			var popularServers = JsonConvert.DeserializeObject<PopularServerJsonModel[]>(response.Body.AsString());
			popularServers.Select(json => json.Endpoint)
				.SequenceEqual(TestData.PopularServers.Take(count))
				.Should()
				.BeTrue();
		}

		[Test]
		[TestCase(2)]
		[TestCase(6)]
		[TestCase(100)]
		[TestCase(-1)]
		public void Get_Reports_BestPlayers(int count)
		{
			var response = count == -1
				? MakeRequest.GetBestPlayers()
				: MakeRequest.GetBestPlayers(count);
			count = ValidateCountParameter(count, TestData.PlayersLeaderboard.Count);

			JsonConvert.DeserializeObject<BestPlayerJsonModel[]>(response.Body.AsString())
				.Select(json => json.Player)
				.SequenceEqual(TestData.PlayersLeaderboard.Take(count))
				.Should()
				.BeTrue();
		}
	}
}