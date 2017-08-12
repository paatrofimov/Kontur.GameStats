using System.Linq;
using Kontur.GameStats.Database;

namespace Kontur.GameStats.Tests.TestHelpers
{
	public static class DbCleaner
	{
		public static void ClearTables()
		{
			using (var ctx = new MyDbContext())
			{
				ctx.Servers.RemoveRange(ctx.Servers.AsEnumerable());
				ctx.Matches.RemoveRange(ctx.Matches.AsEnumerable());
				ctx.Players.RemoveRange(ctx.Players.AsEnumerable());
				ctx.Servers.RemoveRange(ctx.Servers.AsEnumerable());
				ctx.SaveChanges();
			}
		}
	}
}