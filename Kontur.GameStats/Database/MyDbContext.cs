using System.Data.Entity;
using Kontur.GameStats.Models.Entities;
using SQLite.CodeFirst;

namespace Kontur.GameStats.Database
{
	// https://blog.oneunicorn.com/2011/04/15/code-first-inside-dbcontext-initialization/
	public class MyDbContext : DbContext
	{
		public static bool IsForUnitTests { get; set; } = false;

		public DbSet<MatchEntityModel> Matches { get; set; }
		public DbSet<PlayerEntityModel> Players { get; set; }
		public DbSet<ServerEntityModel> Servers { get; set; }

		public MyDbContext() : base("StatserverConnection")
		{
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			// https://blog.oneunicorn.com/2011/03/31/configuring-database-initializers-in-a-config-file/
			if (IsForUnitTests)
			{
				var sqliteDbInitializer = new SqliteDropCreateDatabaseAlways<MyDbContext>(modelBuilder);
				System.Data.Entity.Database.SetInitializer(sqliteDbInitializer);
			}
			else
			{
				var sqliteDbInitializer = new SqliteCreateDatabaseIfNotExists<MyDbContext>(modelBuilder);
				System.Data.Entity.Database.SetInitializer(sqliteDbInitializer);
			}
		}
	}
}