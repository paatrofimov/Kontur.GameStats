using System;
using System.Collections.Generic;
using Kontur.GameStats.Models.Entities;
using Kontur.GameStats.Models.Json;
using Newtonsoft.Json;

namespace Kontur.GameStats.Tests.TestHelpers
{
	public class TestData
	{
		public static List<MatchEntityModel> MatchEntities { get; }
		public static List<DateTime> Timestamps { get; }
		public static List<string> Endpoints { get; }
		public static List<ServerInfoJsonModel> ServerInfoJsons { get; }
		public static List<string> Maps { get; }
		public static List<string> Modes { get; }
		public static List<Record> Records { get; }
		public static List<string> PlayersLeaderboard { get; }
		public static List<string> PopularServers { get; }

		static TestData()
		{
			Maps = new List<string>
			{
				"Map-0",
				"Map-1",
				"Map-2",
				"Map-3",
				"Map-4",
				"Map-5"
			};
			Modes = new List<string>
			{
				"Mode-0",
				"Mode-1",
				"Mode-2",
				"Mode-3",
				"Mode-4",
				"Mode-5"
			};
			PlayersLeaderboard = new List<string>
			{
				"Player-0",
				"Player-1",
				"Player-2"
			};
			Records = new List<Record>
			{
				new Record {Player = "Player-0", Frags = 3, Kills = 3, Deaths = 3},
				new Record {Player = "Player-1", Frags = 2, Kills = 2, Deaths = 2},
				new Record {Player = "Player-2", Frags = 1, Kills = 1, Deaths = 1}
			};
			Endpoints = new List<string>
			{
				"Endpoint-0.0.0.0",
				"Endpoint-1.1.1.1",
				"Endpoint-2.2.2.2"
			};
			PopularServers = new List<string>
			{
				Endpoints[0],
				Endpoints[1],
				Endpoints[2]
			};
			ServerInfoJsons = new List<ServerInfoJsonModel>
			{
				new ServerInfoJsonModel {Name = "ServerName-0", GameModes = new[] {"Mode-0", "Mode-1", "Mode-2"}},
				new ServerInfoJsonModel {Name = "ServerName-1", GameModes = new[] {"Mode-3", "Mode-4"}},
				new ServerInfoJsonModel {Name = "ServerName-2", GameModes = new[] {"Mode-5"}}
			};
			Timestamps = new List<DateTime>
			{
				new DateTime(2017, 5, 6).AddHours(5),
				new DateTime(2017, 5, 6).AddHours(6),
				new DateTime(2016, 7, 8).AddHours(7),
				new DateTime(2016, 7, 8).AddHours(8)
			};
			MatchEntities = new List<MatchEntityModel>
			{
				new MatchEntityModel
				{
					Map = Maps[0],
					GameMode = Modes[0],
					FragLimit = 75,
					TimeLimit = 100,
					TimeElapsed = 125.5,
					Scoreboard = JsonConvert.SerializeObject(new List<Record>
					{
						Records[0],
						Records[1],
						Records[2]
					}),
					TimestampTicks = Timestamps[0].Ticks,
					Endpoint = Endpoints[0]
				},
				new MatchEntityModel
				{
					Map = Maps[0],
					GameMode = Modes[0],
					FragLimit = 100,
					TimeLimit = 125,
					TimeElapsed = 150.5,
					Scoreboard = JsonConvert.SerializeObject(new List<Record>
					{
						Records[0],
						Records[1],
						Records[2],
					}),
					TimestampTicks = Timestamps[1].Ticks,
					Endpoint = Endpoints[0]
				},
				new MatchEntityModel
				{
					Map = Maps[1],
					GameMode = Modes[0],
					FragLimit = 125,
					TimeLimit = 150,
					TimeElapsed = 175.5,
					Scoreboard = JsonConvert.SerializeObject(new List<Record>
					{
						Records[0],
						Records[1],
						Records[2],
					}),
					TimestampTicks = Timestamps[2].Ticks,
					Endpoint = Endpoints[0]
				},
				new MatchEntityModel
				{
					Map = Maps[2],
					GameMode = Modes[1],
					FragLimit = 150,
					TimeLimit = 175,
					TimeElapsed = 200.1,
					Scoreboard = JsonConvert.SerializeObject(new List<Record>
					{
						Records[0],
						Records[1],
						Records[2],
					}),
					TimestampTicks = Timestamps[3].Ticks,
					Endpoint = Endpoints[0]
				},
				new MatchEntityModel
				{
					Map = Maps[2],
					GameMode = Modes[1],
					FragLimit = 175,
					TimeLimit = 200,
					TimeElapsed = 225.1,
					Scoreboard = JsonConvert.SerializeObject(new List<Record>
					{
						Records[0],
						Records[1],
						Records[2],
					}),
					TimestampTicks = Timestamps[0].Ticks,
					Endpoint = Endpoints[1]
				},
				new MatchEntityModel
				{
					Map = Maps[2],
					GameMode = Modes[3],
					FragLimit = 200,
					TimeLimit = 225,
					TimeElapsed = 250.1,
					Scoreboard = JsonConvert.SerializeObject(new List<Record>
					{
						Records[0],
						Records[1],
						Records[2]
					}),
					TimestampTicks = Timestamps[1].Ticks,
					Endpoint = Endpoints[1]
				},
				new MatchEntityModel
				{
					Map = Maps[3],
					GameMode = Modes[3],
					FragLimit = 225,
					TimeLimit = 250,
					TimeElapsed = 275.1,
					Scoreboard = JsonConvert.SerializeObject(new List<Record>
					{
						Records[0],
						Records[1],
						Records[2],
					}),
					TimestampTicks = Timestamps[2].Ticks,
					Endpoint = Endpoints[1]
				},
				new MatchEntityModel
				{
					Map = Maps[4],
					GameMode = Modes[2],
					FragLimit = 250,
					TimeLimit = 275,
					TimeElapsed = 300.1,
					Scoreboard = JsonConvert.SerializeObject(new List<Record>
					{
						Records[0],
						Records[1],
						Records[2],
					}),
					TimestampTicks = Timestamps[0].Ticks,
					Endpoint = Endpoints[2]
				},
				new MatchEntityModel
				{
					Map = Maps[5],
					GameMode = Modes[4],
					FragLimit = 275,
					TimeLimit = 300,
					TimeElapsed = 325.1,
					Scoreboard = JsonConvert.SerializeObject(new List<Record>
					{
						Records[0],
						Records[1],
						Records[2],
					}),
					TimestampTicks = Timestamps[1].Ticks,
					Endpoint = Endpoints[2]
				},
				new MatchEntityModel
				{
					Map = Maps[5],
					GameMode = Modes[5],
					FragLimit = 300,
					TimeLimit = 325,
					TimeElapsed = 350.1,
					Scoreboard = JsonConvert.SerializeObject(new List<Record>
					{
						Records[0],
						Records[1],
						Records[2]
					}),
					TimestampTicks = Timestamps[2].Ticks,
					Endpoint = Endpoints[2]
				},
			};

			MatchWithScoreboardCollision = new MatchEntityModel
			{
				Map = "map",
				GameMode = "CTF",
				FragLimit = 10,
				TimeElapsed = 10,
				TimeLimit = 10,
				Scoreboard = JsonConvert.SerializeObject(new List<Record>
				{
					new Record {Player = "Player0", Frags = 10, Kills = 10, Deaths = 10},
					new Record {Player = "Player0", Frags = 20, Kills = 20, Deaths = 20}
				}),
				TimestampTicks = Timestamps[0].Ticks,
				Endpoint = Endpoints[0]
			};

			MatchWithZeroDeaths = new MatchEntityModel
			{
				Map = "map",
				GameMode = "CTF",
				FragLimit = 10,
				TimeLimit = 10,
				TimeElapsed = 10,
				Scoreboard =
					JsonConvert.SerializeObject(new List<Record>
					{
						new Record {Player = "Player100500", Kills = 10, Deaths = 0, Frags = 10}
					}),
				TimestampTicks = Timestamps[0].Ticks,
				Endpoint = Endpoints[0]
			};
		}

		public static MatchEntityModel MatchWithScoreboardCollision { get; }
		public static MatchEntityModel MatchWithZeroDeaths { get; }
	}
}