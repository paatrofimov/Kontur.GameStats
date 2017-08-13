using System.Linq;
using Kontur.GameStats.Database;
using Kontur.GameStats.Infrastructure.Logging;
using Kontur.GameStats.Models.Json;
using Nancy;
using Nancy.ModelBinding;

namespace Kontur.GameStats.NancyModules
{
	public class ServerInfoModule : NancyModule
	{
		public ServerInfoModule()
		{
			Put["/servers/{endpoint}/info"] = PutServerInfo;
			Get["/servers/{endpoint}/info"] = GetServerInfo;
			Get["/servers/info"] = GetAllServersInfo;
		}

		private object PutServerInfo(dynamic parameters)
		{
			var serverinfo = this.Bind<ServerInfoJsonModel>();
			DbQueries.UpsertServerInfo(parameters.endpoint, serverinfo);
			return HttpStatusCode.OK;
		}

		private object GetServerInfo(dynamic parameters)
		{
			var serverinfo = DbQueries.TrySelectServerInfo(parameters.endpoint);
			if (serverinfo == null)
			{
				MyLogger.LogBadQuery(Request, HttpStatusCode.NotFound,
					LogErrorMessages.MissingServerinfo);
				return HttpStatusCode.NotFound;
			}
			return serverinfo;
		}

		private object GetAllServersInfo(dynamic parameters)
		{
			var serverInfos = DbQueries.SelectAllServerInfos();
			if (!serverInfos.Any())
			{
				MyLogger.LogBadQuery(Request, HttpStatusCode.NotFound,
					LogErrorMessages.MissingAnyServerinfo);
				return Response.AsJson("", HttpStatusCode.NotFound);
			}
			return serverInfos;
		}
	}
}