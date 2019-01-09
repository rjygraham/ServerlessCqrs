using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using ServerlessCqrs.Domain.Events;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ServerlessCqrs.WebJobs.Extensions.Services.Events
{
	public class EventGridEventPublisher : IEventPublisher
	{
		private readonly EventGridClient client;
		private readonly string topicHostName;

		public EventGridEventPublisher(EventGridClient client, string topicHostName)
		{
			this.client = client;
			this.topicHostName = topicHostName;
		}

		public async Task PublishEvents(IEnumerable<IEvent> events, CancellationToken cancellationToken = default)
		{
			var wrappedEvents = events.Select(s => new EventGridEvent(s.Id.ToString(), s.AggregateId.ToString(), s, s.GetType().Name, s.TimeStamp.UtcDateTime, s.Version.ToString())).ToList();
			await client.PublishEventsAsync(topicHostName, wrappedEvents, cancellationToken);
		}
	}
}
