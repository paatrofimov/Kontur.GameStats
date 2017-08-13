using System;
using Kontur.GameStats.Infrastructure;
using log4net.Config;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Newtonsoft.Json;

namespace Kontur.GameStats.Entry
{
	public class Bootstrapper : DefaultNancyBootstrapper
	{
		protected override void ConfigureApplicationContainer(TinyIoCContainer container)
		{
			base.ConfigureApplicationContainer(container);
			XmlConfigurator.Configure();
			container.Register<JsonSerializer, CamelCaseJsonSerializer>();
		}

		protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
		{
			StaticConfiguration.DisableErrorTraces = false;
			pipelines.BeforeRequest += context =>
			{
				context.Request.Headers.Accept = new[] {Tuple.Create("application/json", 1m)};
				context.Request.Headers.ContentType = "application/json";
				return null;
			};
			pipelines.OnError += (ctx, ex) => HttpStatusCode.InternalServerError;
		}
	}
}