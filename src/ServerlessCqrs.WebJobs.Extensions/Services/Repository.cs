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

using ServerlessCqrs.Domain;
using ServerlessCqrs.Domain.Exceptions;
using ServerlessCqrs.WebJobs.Extensions.Services.Events;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ServerlessCqrs.WebJobs.Extensions.Services
{
	internal class Repository: IRepository
	{
		private readonly IEventStore eventStore;
		private readonly IEventPublisher eventPublisher;

		internal Repository(IEventStore eventStore, IEventPublisher eventPublisher)
		{
			this.eventStore = eventStore;
			this.eventPublisher = eventPublisher;
		}

		public Task<TAggregate> GetAsync<TAggregate>(Guid aggregateId, CancellationToken cancellationToken = default) where TAggregate : AggregateRoot, new()
		{
			return LoadAggregateAsync<TAggregate>(aggregateId, cancellationToken);
		}

		public async Task SaveAsync(AggregateRoot aggregate, int? expectedVersion, CancellationToken cancellationToken)
		{
			if (expectedVersion != null && (await eventStore.GetAsync(aggregate.Id, expectedVersion.Value, aggregate.PartitionKey, cancellationToken).ConfigureAwait(false)).Any())
			{
				throw new ConcurrencyException(aggregate.Id);
			}

			var changes = aggregate.FlushUncommittedChanges();

			await eventStore.SaveAsync(changes, cancellationToken).ConfigureAwait(false);
			await eventPublisher.PublishEvents(changes, cancellationToken).ConfigureAwait(false);
		}

		private async Task<TAggregate> LoadAggregateAsync<TAggregate>(Guid aggregateId, CancellationToken cancellationToken = default) where TAggregate : AggregateRoot, new()
		{
			var aggregateType = typeof(TAggregate);

			var events = await eventStore.GetAsync(aggregateId, -1, $"{aggregateType.Name}|{aggregateId}", cancellationToken).ConfigureAwait(false);
			if (!events.Any())
			{
				throw new AggregateNotFoundException(typeof(TAggregate), aggregateId);
			}

			var aggregate = new TAggregate();
			aggregate.LoadFromHistory(events);
			return aggregate;
		}
	}
}
