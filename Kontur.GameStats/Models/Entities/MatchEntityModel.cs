using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kontur.GameStats.Models.Entities
{
	public class MatchEntityModel
	{
		public MatchEntityModel()
		{
		}

		[Key, Column(Order = 1)]
		public string Endpoint { get; set; }

		[Key, Column(Order = 2)]
		public long TimestampTicks { get; set; }

		public string Map { get; set; }
		public string GameMode { get; set; }
		public int FragLimit { get; set; }
		public int TimeLimit { get; set; }
		public double TimeElapsed { get; set; }
		public string Scoreboard { get; set; }
	}
}