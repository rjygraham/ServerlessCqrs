using ServerlessCqrs.Domain.Events;
using ServerlessCqrs.Sample.Domain.ValueTypes;
using System;

namespace ServerlessCqrs.Sample.Domain.Events
{
	public class PersonCreated : EventBase
	{
		public string GivenName { get; set; }
		public string Surname { get; set; }
		public DateTime Dob { get; set; }
		public Gender Gender { get; set; }
		
		public PersonCreated() { }

		public PersonCreated(Guid aggregateId, string givenName, string surname, DateTime dob, Gender gender)
			: base(aggregateId)
		{
			GivenName = givenName;
			Surname = surname;
			Dob = dob;
			Gender = gender;
		}
	}
}
