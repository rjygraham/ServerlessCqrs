// Copyright 2018 Gaute Magnussen

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//	http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// NOTE: Portions of this file may have been modified from their original
// source found here: https://github.com/gautema/CQRSlite/

using ServerlessCqrs.Domain.Events;
using ServerlessCqrs.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerlessCqrs.Domain
{
	public abstract class AggregateRoot
	{
		private readonly IList<IEvent> changes = new List<IEvent>();

		public Guid Id { get; protected set; }
		public int Version { get; set; }
		public abstract string PartitionKey { get; }

		public IEvent[] FlushUncommittedChanges()
		{
			lock (changes)
			{
				var result = changes.ToArray();
				var i = 0;
				foreach (var @event in result)
				{
					if (@event.AggregateId == Guid.Empty && Id == Guid.Empty)
					{
						throw new AggregateOrEventMissingIdException(GetType(), @event.GetType());
					}
					if (@event.AggregateId == Guid.Empty)
					{
						@event.AggregateId = Id;
					}
					i++;

					@event.PartitionKey = PartitionKey;
					@event.Version = Version + i;
					@event.TimeStamp = DateTimeOffset.UtcNow;
				}

				Version = Version + result.Length;
				changes.Clear();
				return result;
			}
		}

		public void LoadFromHistory(IEnumerable<IEvent> history)
		{
			lock (changes)
			{
				foreach (var e in history.ToArray())
				{
					if (e.Version != Version + 1)
					{
						throw new EventsOutOfOrderException(e.AggregateId);
					}
					ApplyEvent(e);
					Id = e.AggregateId;
					Version++;
				}
			}
		}

		protected void ApplyChange(IEvent @event)
		{
			lock (changes)
			{
				ApplyEvent(@event);
				changes.Add(@event);
			}
		}

		protected abstract void ApplyEvent(IEvent @event);
	}
}
