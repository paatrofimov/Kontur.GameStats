using System;
using System.Linq;
using Fclp;
using Microsoft.Owin.Hosting;
using Owin;

namespace Kontur.GameStats.Entry
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var commandLineParser = new FluentCommandLineParser<Options>();
			commandLineParser
				.Setup(options => options.Prefix)
				.As("prefix")
				//.SetDefault("http://+:8080/") // todo почему-то не работает
				.WithDescription("HTTP prefix to listen on");
			commandLineParser
				.Setup(options => options.DbName)
				.As("dbname")
				//.SetDefault("statserver.db")
				.WithDescription("sqlite database name to store data in");
			commandLineParser
				.SetupHelp("h", "help")
				.WithHeader(
					$"{AppDomain.CurrentDomain.FriendlyName} [--prefix <prefix> like http://1.1.1.1:1234/] [--dbname <dbname> like name.db]")
				.Callback(text => Console.WriteLine(text));
			if (args.Any() && commandLineParser.Parse(args).HelpCalled)
			{
				WaitForUserShutdown();
				return;
			}
			RunServer(commandLineParser.Object);
		}

		private static void RunServer(Options options)
		{
			using (WebApp.Start(options.Prefix, builder => builder.UseNancy(opts => opts.Bootstrapper = new Bootstrapper())))
			{
				Console.WriteLine("Starting stat server with \r\n" +
				                  $"\t database '{options.DbName}' \r\n" +
				                  $"\t http prefix '{options.Prefix}' \r\n");
				WaitForUserShutdown();
			}
		}

		private static void WaitForUserShutdown()
		{
			Console.WriteLine("Press any key to shut down app \r\n");
			Console.ReadLine();
		}
	}
}