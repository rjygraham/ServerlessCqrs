using Microsoft.Azure.WebJobs;
using ServerlessCqrs.WebJobs.Extensions.Services.Events;
using ServerlessCqrs.WebJobs.Extensions.Utils;

namespace ServerlessCqrs.WebJobs.Extensions.Bindings
{
	internal class EventWebhookHandlerBuilder : IConverter<ServerlessCqrsEventWebhookHandlerAttribute, IEventWebhookHandler>
	{
		internal EventWebhookHandlerBuilder()
		{
		}

		public IEventWebhookHandler Convert(ServerlessCqrsEventWebhookHandlerAttribute attribute)
		{
			EventTypeHandlerMapping.InitializedMapping();
			return new EventWebhookHandler(attribute);
		}
	}
}
