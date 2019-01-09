using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServerlessCqrs.Domain.Events;
using ServerlessCqrs.WebJobs.Extensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ServerlessCqrs.WebJobs.Extensions.Services.Events
{
	internal class EventWebhookHandler : IEventWebhookHandler
	{
		private readonly ServerlessCqrsEventWebhookHandlerAttribute attribute;

		internal EventWebhookHandler(ServerlessCqrsEventWebhookHandlerAttribute attribute)
		{
			this.attribute = attribute;
		}

		public async Task<HttpResponseMessage> HandleAsync(HttpRequestMessage request, DurableOrchestrationClient durableClient)
		{
			var events = await request.Content.ReadAsAsync<IList<EventGridEvent>>();

			// If the request is for subscription validation, send back the validation code.
			if (events.Count > 0 && string.Equals(events[0].EventType, ServerlessCqrsConstants.EventGridValidationEventType, StringComparison.OrdinalIgnoreCase))
			{
				return request.CreateResponse<object>(new { validationResponse = ((dynamic)events[0].Data).validationCode });
			}

			string instanceId = await durableClient.StartNewAsync(attribute.EventHandlerOrchestratorName, events);
			return request.CreateResponse<object>(new { instanceId });
		}
	}
}
