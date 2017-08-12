using System;
using System.Linq;
using FluentAssertions;
using Kontur.GameStats.Database;
using Kontur.GameStats.Infrastructure.Logging;
using Kontur.GameStats.Models.Json;
using Kontur.GameStats.Tests.TestHelpers;
using Nancy;
using Nancy.Testing;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Kontur.GameStats.Tests
{
	[TestFixture]
	public class Edgecases_Statserver_Should
	{
		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			MyDbContext.IsForUnitTests = true;
		}

		[TearDown]
		public void TearDown()
		{
			DbCleaner.ClearTables();
		}

		[Test]
		public void Respond_With_BadRequest_When_Put_Match_To_Not_Advertised_Endpoint()
		{
			var response = MakeRequest.PutMatch(TestData.MatchEntities[0]);

			response.Body.AsString().Should().Be(string.Empty);
			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Test]
		public void Log_BadRequest_When_Put_Match_To_Not_Advertised_Endpoint()
		{
			MakeRequest.PutMatch(TestData.MatchEntities[0]);

			ConsoleLogger.Events.Last()
				.Should()
				.Be(
					$"Bad query: PUT {MakeRequest.LastRequestUrl} \r\n" +
					"\t responded with http code 'BadRequest' \r\n" +
					$"Message: {LogErrorMessages.AdvertisingError} \r\n\r\n"
				);
		}

		[Test]
		public void Respond_With_NotFound_When_Get_Not_Advertised_ServerInfo()
		{
			var response = MakeRequest.GetServerInfo(TestData.Endpoints[0]);

			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Test]
		public void Log_NotFound_When_Get_Not_Advertised_ServerInfo()
		{
			MakeRequest.GetServerInfo(TestData.Endpoints[0]);

			ConsoleLogger.Events.Last()
				.Should()
				.Be(
					$"Bad query: GET {MakeRequest.LastRequestUrl} \r\n" +
					"\t responded with http code 'NotFound' \r\n" +
					$"Message: {LogErrorMessages.MissingServerinfo} \r\n\r\n"
				);
		}

		[Test]
		public void Respond_With_NotFound_When_Get_All_ServerInfo_With_No_Any_Advertised_Sever()
		{
			var response = MakeRequest.GetAllServerInfos();

			response.Body.AsString().Should().Be("\"\"");
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Test]
		public void Log_NotFound_When_Get_All_ServerInfo_With_No_Any_Advertised_Sever()
		{
			MakeRequest.GetAllServerInfos();

			ConsoleLogger.Events.Last()
				.Should()
				.Be(
					$"Bad query: GET {MakeRequest.LastRequestUrl} \r\n" +
					"\t responded with http code 'NotFound' \r\n" +
					$"Message: {LogErrorMessages.MissingAnyServerinfo} \r\n\r\n"
				);
		}

		[Test]
		public void Respond_With_MethodNotAllowed_When_Put_Same_Match_Twice()
		{
			MakeRequest.PutServerInfo(TestData.Endpoints[0], TestData.ServerInfoJsons[0]);
			MakeRequest.PutMatch(TestData.MatchEntities[0]);

			var response = MakeRequest.PutMatch(TestData.MatchEntities[0]);

			response.Body.AsString().Should().Be(string.Empty);
			response.StatusCode.ShouldBeEquivalentTo((int) HttpStatusCode.MethodNotAllowed);
		}

		[Test]
		public void Log_MethodNotAllowed_When_Put_Same_Match_Twice()
		{
			MakeRequest.PutServerInfo(TestData.Endpoints[0], TestData.ServerInfoJsons[0]);
			MakeRequest.PutMatch(TestData.MatchEntities[0]);
			MakeRequest.PutMatch(TestData.MatchEntities[0]);

			ConsoleLogger.Events.Last()
				.Should()
				.Be(
					$"Bad query: PUT {MakeRequest.LastRequestUrl} \r\n" +
					"\t responded with http code 'MethodNotAllowed' \r\n" +
					$"Message: {LogErrorMessages.MatchesCollision} \r\n\r\n"
				);
		}

		[Test]
		public void Respond_With_MethodNotAllowed_When_Put_Match_With_Scoreboard_With_Collision()
		{
			MakeRequest.PutServerInfo(TestData.Endpoints[0], TestData.ServerInfoJsons[0]);
			var response = MakeRequest.PutMatch(TestData.MatchWithScoreboardCollision);

			response.Body.AsString().Should().Be(string.Empty);
			response.StatusCode.ShouldBeEquivalentTo((int) HttpStatusCode.MethodNotAllowed);
		}

		[Test]
		public void Log_MethodNotAllowed_When_Put_Match_With_Scoreboard_Collision()
		{
			MakeRequest.PutServerInfo(TestData.Endpoints[0], TestData.ServerInfoJsons[0]);
			MakeRequest.PutMatch(TestData.MatchWithScoreboardCollision);

			ConsoleLogger.Events.Last()
				.Should()
				.Be(
					$"Bad query: PUT {MakeRequest.LastRequestUrl} \r\n" +
					"\t responded with http code 'MethodNotAllowed' \r\n" +
					$"Message: {LogErrorMessages.ScoreboardCollision} \r\n\r\n"
				);
		}

		[Test]
		public void Count_Kd_When_Deaths_Are_0()
		{
			MakeRequest.PutServerInfo(TestData.Endpoints[0], TestData.ServerInfoJsons[0]);
			MakeRequest.PutMatch(TestData.MatchWithZeroDeaths);
			var response = MakeRequest.GetPlayer(
				new MatchJsonModel(TestData.MatchWithZeroDeaths).Scoreboard.First().Player);

			var playerJson = JsonConvert.DeserializeObject<PlayerJsonModel>(response.Body.AsString());
			playerJson.KillToDeathRatio.Should().BeApproximately(10, 1e-7);
		}

		[Test]
		public void Count_Scoreboard_Percent_When_Single_Record()
		{
			MakeRequest.PutServerInfo(TestData.Endpoints[0], TestData.ServerInfoJsons[0]);
			MakeRequest.PutMatch(TestData.MatchWithZeroDeaths);
			var response = MakeRequest.GetPlayer(
				new MatchJsonModel(TestData.MatchWithZeroDeaths).Scoreboard.First().Player);

			var playerJson = JsonConvert.DeserializeObject<PlayerJsonModel>(response.Body.AsString());
			playerJson.AverageScoreboardPercent.Should().BeApproximately(100, 1e-7);
		}

		[Test]
		public void Rewrite_Serverinfo_With_Same_Endpoint()
		{
			MakeRequest.PutServerInfo(TestData.Endpoints[0], TestData.ServerInfoJsons[2]);
			MakeRequest.PutServerInfo(TestData.Endpoints[0], TestData.ServerInfoJsons[0]);

			var response = MakeRequest.GetServerInfo(TestData.Endpoints[0]);

			var serverInfoJson = JsonConvert.DeserializeObject<ServerInfoJsonModel>(response.Body.AsString());
			serverInfoJson.Should()
				.Be(new ServerInfoJsonModel
				{
					GameModes = TestData.ServerInfoJsons[0].GameModes,
					Server = TestData.ServerInfoJsons[0].Server
				});
		}

		[Test]
		public void Get_Default_ServerStats_When_No_Match_Were_Played_On_Server()
		{
			MakeRequest.PutServerInfo(TestData.Endpoints[0], TestData.ServerInfoJsons[0]);
			var response = MakeRequest.GetServerStats(TestData.Endpoints[0]);

			JsonConvert.DeserializeObject<ServerStatsJsonModel>(response.Body.AsString())
				.Should()
				.Be(new ServerStatsJsonModel());
		}

		private readonly string[] incorrect =
		{
			"http://localhost:8080/",
			"http://localhost:8080/servers",
			"http://localhost:8080/server/info",
			"http://localhost:8080/servers/192.1.1.1-1111/matches/2017-01-22T15:17:00",
			"http://localhost:8080/servers/192.1.1.1-1111/matches/207-01-22T15:17:00",
			"http://localhost:8080/servers/192.1.1.1-1111/matches/",
			"http://localhost:8080/servers/192.1.1.1-1111/matches",
			"http://localhost:8080/servers/192.1.1.1-1111/mat",
			"http://localhost:8080/servers//matches/2017-01-22T15:17:00Z",
			"http://localhost:8080/servers/matches/2017-01-22T15:17:00Z",
			"http://localhost:8080/server/192.1.1.1-1111/matches/2017-01-22T15:17:00",
			"http://localhost:8080/192.1.1.1-1111/matches/2017-01-22T15:17:00",
			"http://localhost:8080/reports/recent-matches/-1",
			"http://localhost:8080/reports/recent-matches/sup",
			"http://localhost:8080/reports/recent_matches/1",
			"http://localhost:8080/reports/recent_matches/1/",
			"http://localhost:8080/reports/1/recent_matches",
			"http://localhost:8080/reports/1/"
		};

		[Test]
		public void Process_Incorrect_Put_Queries()
		{
			incorrect.All(r =>
				{
					var response = MakeRequest.MakePutRequest(r, "");
					if (response.StatusCode == HttpStatusCode.BadRequest ||
					    response.StatusCode == HttpStatusCode.NotFound ||
					    response.StatusCode == HttpStatusCode.MethodNotAllowed)
						return true;
					Console.WriteLine(r);
					return false;
				})
				.Should()
				.BeTrue();
		}

		[Test]
		public void Process_Incorrect_Get_Queries()
		{
			incorrect.All(r =>
				{
					var response = MakeRequest.MakeGetRequest(r);
					if (response.StatusCode == HttpStatusCode.BadRequest ||
					    response.StatusCode == HttpStatusCode.NotFound ||
					    response.StatusCode == HttpStatusCode.MethodNotAllowed)
						return true;
					Console.WriteLine(r);
					return false;
				})
				.Should()
				.BeTrue();
		}
	}
}