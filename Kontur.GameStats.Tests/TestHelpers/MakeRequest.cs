using System;
using Kontur.GameStats.Entry;
using Kontur.GameStats.Infrastructure;
using Kontur.GameStats.Models.Entities;
using Kontur.GameStats.Models.Json;
using Nancy.Testing;
using Newtonsoft.Json;

namespace Kontur.GameStats.Tests.TestHelpers
{
	public static class MakeRequest
	{
		private static Browser browser = new Browser(new Bootstrapper());
		private static string Prefix = "http://localhost:8080/";
		public static string LastRequestUrl { get; private set; }

		public static BrowserResponse PutServerInfo(string endpoint, ServerInfoJsonModel serverInfo)
			=> SerializeAndMakePutRequest($"{Prefix}servers/{endpoint}/info", serverInfo);

		public static BrowserResponse PutMatch(MatchEntityModel matchEntity)
		{
			var matchDate = new DateTime(matchEntity.TimestampTicks).ToIso8601();
			var body = new MatchJsonModel(matchEntity);
			return SerializeAndMakePutRequest($"{Prefix}servers/{matchEntity.Endpoint}/matches/{matchDate}", body);
		}

		private static BrowserResponse SerializeAndMakePutRequest(string url, object requestObj)
			=> MakePutRequest(url, JsonConvert.SerializeObject(requestObj));

		public static BrowserResponse MakePutRequest(string url, string request)
		{
			LastRequestUrl = url;
			return browser.Put(url, context => context.Body(request));
		}

		public static BrowserResponse GetServerInfo(string endpoint)
			=> MakeGetRequest($"{Prefix}servers/{endpoint}/info");

		public static BrowserResponse GetAllServerInfos()
			=> MakeGetRequest($"{Prefix}servers/info");

		public static BrowserResponse GetMatch(string endpoint, DateTime timestamp)
			=> MakeGetRequest($"{Prefix}servers/{endpoint}/matches/{timestamp.ToIso8601()}");

		public static BrowserResponse GetServerStats(string endpoint)
			=> MakeGetRequest($"{Prefix}servers/{endpoint}/stats");

		public static BrowserResponse GetPlayer(string encodedPlayer)
			=> MakeGetRequest($"{Prefix}players/{encodedPlayer}/stats");

		public static BrowserResponse GetRecentMatches(int count)
			=> MakeGetRequest($"{Prefix}reports/recent-matches/{count}");

		public static BrowserResponse GetRecentMatches()
			=> MakeGetRequest($"{Prefix}reports/recent-matches");

		public static BrowserResponse GetBestPlayers(int count)
			=> MakeGetRequest($"{Prefix}reports/best-players/{count}");

		public static BrowserResponse GetBestPlayers()
			=> MakeGetRequest($"{Prefix}reports/best-players");

		public static BrowserResponse GetPopularServers(int count)
			=> MakeGetRequest($"{Prefix}reports/popular-servers/{count}");

		public static BrowserResponse GetPopularServers()
			=> MakeGetRequest($"{Prefix}reports/popular-servers");

		public static BrowserResponse MakeGetRequest(string url)
		{
			LastRequestUrl = url;
			return browser.Get(url);
		}
	}
}