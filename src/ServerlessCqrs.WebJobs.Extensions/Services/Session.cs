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
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ServerlessCqrs.WebJobs.Extensions.Services
{
	internal class Session : ISession
	{
		private readonly IRepository repository;
		private readonly IDictionary<Guid, AggregateRoot> trackedAggregates;

		internal Session(IRepository repository)
		{
			this.repository = repository;
			trackedAggregates = new Dictionary<Guid, AggregateRoot>();
		}

		public Task AddAsync<TAggregateRoot>(TAggregateRoot aggregate, CancellationToken cancellationToken = default) where TAggregateRoot : AggregateRoot
		{
			if (!trackedAggregates.ContainsKey(aggregate.Id))
			{
				trackedAggregates.Add(aggregate.Id, aggregate);
			}
			else if (trackedAggregates[aggregate.Id] != aggregate)
			{
				throw new ConcurrencyException(aggregate.Id);
			}

			return Task.FromResult(0);
		}

		public async Task CommitChangesAsync(CancellationToken cancellationToken = default)
		{
			var tasks = new Task[trackedAggregates.Count];
			var i = 0;

			foreach (var aggregate in trackedAggregates.Values)
			{
				tasks[i] = repository.SaveAsync(aggregate, aggregate.Version, cancellationToken);
				i++;
			}

			await Task.WhenAll(tasks).ConfigureAwait(false);

			trackedAggregates.Clear();
		}

		public async Task<TAggregate> GetAsync<TAggregate>(Guid id, int? expectedVersion, CancellationToken cancellationToken = default) where TAggregate : AggregateRoot, new()
		{
			if (trackedAggregates.ContainsKey(id))
			{
				var trackedAggregate = (TAggregate)trackedAggregates[id];

				if (expectedVersion != null && trackedAggregate.Version != expectedVersion)
				{
					throw new ConcurrencyException(trackedAggregate.Id);
				}
				return trackedAggregate;
			}

			var aggregate = await repository.GetAsync<TAggregate>(id, cancellationToken).ConfigureAwait(false);
	
			if (expectedVersion != null && aggregate.Version != expectedVersion)
			{
				throw new ConcurrencyException(id);
			}

			await AddAsync(aggregate, cancellationToken).ConfigureAwait(false);

			return aggregate;
		}
	}
}
