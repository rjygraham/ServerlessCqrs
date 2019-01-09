using System;

namespace ServerlessCqrs.Domain.Events
{
	public abstract class EventBase : IEvent
	{
		public Guid Id { get; set; }
		public Guid AggregateId { get; set; }
		public int Version { get; set; }
		public DateTimeOffset TimeStamp { get; set; }
		public string PartitionKey { get; set; }

		public EventBase() { }

		public EventBase(Guid aggregateId)
		{
			Id = Guid.NewGuid();
			AggregateId = aggregateId;
		}
	}
}
