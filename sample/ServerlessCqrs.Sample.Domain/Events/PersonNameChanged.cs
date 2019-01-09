using ServerlessCqrs.Domain.Events;
using System;

namespace ServerlessCqrs.Sample.Domain.Events
{
	public class PersonNameChanged : EventBase
	{
		public string NewGivenName { get; set; }
		public string NewSurname { get; set; }

		public PersonNameChanged() { }

		public PersonNameChanged(Guid aggregateId, string newGivenName, string newSurname)
			: base(aggregateId)
		{
			NewGivenName = newGivenName;
			NewSurname = newSurname;
		}
	}
}
