using System.Collections.Generic;
using System.IO;
using Nancy;
using Nancy.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kontur.GameStats.Infrastructure
{
	public sealed class CamelCaseJsonSerializer : JsonSerializer, ISerializer
	{
		private DefaultJsonSerializer defaultJsonSerializer = new DefaultJsonSerializer();

		public CamelCaseJsonSerializer()
		{
			Formatting = Formatting.Indented;
			ContractResolver = new CamelCasePropertyNamesContractResolver();
			DateFormatHandling = DateFormatHandling.IsoDateFormat;
		}

		public bool CanSerialize(string contentType) => defaultJsonSerializer.CanSerialize(contentType);

		public IEnumerable<string> Extensions => defaultJsonSerializer.Extensions;

		public void Serialize<TModel>(string contentType, TModel model, Stream outputStream)
		{
			using (var streamWriter = new StreamWriter(outputStream))
			using (var jsonWriter = new JsonTextWriter(streamWriter))
			{
				Serialize(jsonWriter, model);
			}
		}
	}
}