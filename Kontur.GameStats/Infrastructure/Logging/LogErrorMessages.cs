using Kontur.GameStats.Properties;

namespace Kontur.GameStats.Infrastructure.Logging
{
	public static class LogErrorMessages
	{
		public static string AdvertisingError { get; } = Resources.AdvertisingErrorMsg;
		public static string MatchesCollision { get; } = Resources.MatchesCollisionMsg;
		public static string ScoreboardCollision { get; } = Resources.ScoreboardCollisionMsg;
		public static string MissingMatch { get; } = Resources.MissingMatchMsg;
		public static string MissingPlayer { get; } = Resources.MissingPlayerMsg;
		public static string MissingServerinfo { get; } = Resources.MissingServerinfo;
		public static string MissingAnyServerinfo { get; } = Resources.MissingAnyServerinfo;
		public static string MissingServerstats { get; } = Resources.MissingServerstats;
	}
}