using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Kontur.GameStats.Infrastructure;
using Kontur.GameStats.Models.Json;
using Newtonsoft.Json;

namespace Kontur.GameStats.Models.Entities
{
	public class PlayerEntityModel
	{
		public PlayerEntityModel()
		{
		}

		public void Update(MatchJsonModel matchJson, DateTime matchTimestamp, string endpoint)
		{
			SetDictionaryHelpers(matchJson, matchTimestamp, endpoint);
			LatestMatchInTicks = Math.Max(LatestMatchInTicks, matchTimestamp.Ticks);
			TotalMatchesPlayed++;
			var leaderboard = matchJson.Scoreboard.OrderByDescending(record => record.Frags).ToList();
			var playerInd = leaderboard.FindIndex(record => record.Player == Player);
			TotalKills += leaderboard[playerInd].Kills;
			TotalDeaths += leaderboard[playerInd].Deaths;
			var scoreboardPercent = leaderboard.Count == 1 ? 100.0 : 100.0 - playerInd / (leaderboard.Count - 1.0);
			TotalScoreboardPercent += scoreboardPercent;
			if (Math.Abs(scoreboardPercent - 100) < 1e-7)
				TotalMatchesWon++;
		}

		private void SetDictionaryHelpers(MatchJsonModel matchJson, DateTime matchTimestamp, string endpoint)
		{
			var matchesPerServer = JsonConvert.DeserializeObject<Dictionary<string, int>>(MatchesPerServer) ??
			                       new Dictionary<string, int>();
			var matchesPerDate = JsonConvert.DeserializeObject<Dictionary<DateTime, int>>(MatchesPerDate) ??
			                     new Dictionary<DateTime, int>();
			var matchesPerMode = JsonConvert.DeserializeObject<Dictionary<string, int>>(MatchesPerMode) ??
			                     new Dictionary<string, int>();
			matchesPerServer.AddOrIncrement(endpoint);
			matchesPerMode.AddOrIncrement(matchJson.GameMode);
			matchesPerDate.AddOrIncrement(matchTimestamp.Date);
			MatchesPerServer = JsonConvert.SerializeObject(matchesPerServer);
			MatchesPerMode = JsonConvert.SerializeObject(matchesPerMode);
			MatchesPerDate = JsonConvert.SerializeObject(matchesPerDate);
		}

		public double CountKillToDeathRatio()
		{
			return TotalDeaths == 0 ? TotalKills : 1.0 * TotalKills / TotalDeaths;
		}

		[Key]
		public string Player { get; set; }

		public string MatchesPerServer { get; set; } = String.Empty;
		public string MatchesPerMode { get; set; } = String.Empty;
		public string MatchesPerDate { get; set; } = String.Empty;
		public int TotalMatchesPlayed { get; set; }
		public int TotalMatchesWon { get; set; }
		public int TotalKills { get; set; }
		public int TotalDeaths { get; set; }
		public long LatestMatchInTicks { get; set; }
		public double TotalScoreboardPercent { get; set; }
	}
}