using Kontur.GameStats.Models.Entities;

namespace Kontur.GameStats.Models.Json
{
	public class BestPlayerJsonModel
	{
		public BestPlayerJsonModel()
		{
		}

		public BestPlayerJsonModel(PlayerEntityModel entity)
		{
			Player = entity.Player;
			KillToDeathRatio = entity.CountKillToDeathRatio();
		}

		public string Player { get; set; }
		public double KillToDeathRatio { get; set; }
	}
}