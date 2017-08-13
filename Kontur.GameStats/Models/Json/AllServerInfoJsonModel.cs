using Kontur.GameStats.Models.Entities;
using Newtonsoft.Json;

namespace Kontur.GameStats.Models.Json
{
	public class AllServerInfoJsonModel
	{
		public AllServerInfoJsonModel()
		{
		}

		public AllServerInfoJsonModel(ServerEntityModel entity)
		{
			Endpoint = entity.Endpoint;
			Info = JsonConvert.SerializeObject(new ServerInfoJsonModel(entity));
		}

		public string Endpoint { get; set; }
		public string Info { get; set; }
	}
}