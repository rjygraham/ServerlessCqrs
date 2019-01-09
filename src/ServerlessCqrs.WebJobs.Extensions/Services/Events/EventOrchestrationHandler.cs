using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using ServerlessCqrs.WebJobs.Extensions.Utils;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ServerlessCqrs.WebJobs.Extensions.Services.Events
{
	internal class EventOrchestrationHandler : IEventOrchestrationHandler
	{
		public async Task<bool> HandleAsync(DurableOrchestrationContext context, CancellationToken cancellationToken = default)
		{
			var events = context.GetInput<IList<EventGridEvent>>();

			foreach (var @event in events)
			{
				var handlers = EventTypeHandlerMapping.GetEventHandlers(@event.EventType);

				if (handlers?.Count > 0)
				{
					var tasks = new List<Task>();

					foreach (var handler in handlers)
					{
						tasks.Add(context.CallActivityAsync(handler, @event.Data));
					}
					
					await Task.WhenAll(tasks);
				}
			}

			return true;
		}
	}
}
