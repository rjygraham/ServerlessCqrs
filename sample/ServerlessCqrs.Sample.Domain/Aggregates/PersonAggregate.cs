using ServerlessCqrs.Domain;
using ServerlessCqrs.Domain.Events;
using ServerlessCqrs.Domain.Exceptions;
using ServerlessCqrs.Sample.Domain.Events;
using ServerlessCqrs.Sample.Domain.ValueTypes;
using System;

namespace ServerlessCqrs.Sample.Domain.Aggregates
{
	public class PersonAggregate : AggregateRoot
	{
		private string givenName;
		private string surname;
		DateTime dob;
		Gender gender;

		public override string PartitionKey { get => $"{nameof(PersonAggregate)}|{Id}"; }

		public PersonAggregate() { }

		public PersonAggregate(Guid id, string givenName, string surname, DateTime dob, Gender gender)
		{
			Id = id;
			ApplyChange(new PersonCreated(id, givenName, surname, dob, gender));
		}

		private void Apply(PersonCreated @event)
		{
			givenName = @event.GivenName;
			surname = @event.Surname;
			dob = @event.Dob;
			gender = @event.Gender;
		}

		public void ChangeName(string newGivenName, string newSurname)
		{
			ApplyChange(new PersonNameChanged(Id, newGivenName, newSurname));
		}

		private void Apply(PersonNameChanged @event)
		{
			givenName = @event.NewGivenName;
			surname = @event.NewSurname;
		}

		protected override void ApplyEvent(IEvent @event)
		{
			switch (@event)
			{
				case PersonCreated personCreated: Apply(personCreated); return;
				case PersonNameChanged nameChanged: Apply(nameChanged); return;
				default:
					throw new AggregateMissingApplyException(GetType(), @event.GetType());
			}
		}
		
	}
}
