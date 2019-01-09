using System;

namespace ServerlessCqrs.Domain.Events
{
	public interface IEvent
	{
		Guid Id { get; set; }
		Guid AggregateId { get; set; }
		int Version { get; set; }
		DateTimeOffset TimeStamp { get; set; }
		string PartitionKey { get; set; }
	}
}
