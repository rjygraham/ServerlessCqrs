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

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ServerlessCqrs.Domain.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ServerlessCqrs.WebJobs.Extensions.Services.Events
{
	public class CosmosDbEventStore : IEventStore
	{
		private readonly DocumentClient client;
		private readonly Uri documentCollectionUri;

		public CosmosDbEventStore(DocumentClient client, string databaseId, string collectionId)
		{
			documentCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);
			this.client = client;
		}

		public async Task<IEnumerable<IEvent>> GetAsync(Guid aggregateId, int fromVersion, string partitionKey, CancellationToken cancellationToken)
		{
			var sqlQuery = new SqlQuerySpec($"SELECT * FROM e WHERE e.aggregateId = @AggregateId AND e.version > @Version");
			sqlQuery.Parameters.Add(new SqlParameter("@AggregateId", aggregateId));
			sqlQuery.Parameters.Add(new SqlParameter("@Version", fromVersion));

			return await GetAsync(sqlQuery, partitionKey, cancellationToken);
		}

		private async Task<IEnumerable<IEvent>> GetAsync(SqlQuerySpec sqlQuery, string partitionKey, CancellationToken cancellationToken)
		{
			var result = new HashSet<IEvent>();

			try
			{
				var serializerSettings = new JsonSerializerSettings
				{
					ContractResolver = new CamelCasePropertyNamesContractResolver(),
					TypeNameHandling = TypeNameHandling.Objects
				};

				var query = client.CreateDocumentQuery<IEvent>(documentCollectionUri, sqlQuery, new FeedOptions { PartitionKey = new PartitionKey(partitionKey) })
					.AsDocumentQuery();

				while (query.HasMoreResults)
				{
					foreach (var e in await query.ExecuteNextAsync<IEvent>(cancellationToken))
					{
						result.Add(e as IEvent);
					}
				}
			}
			catch (Exception ex)
			{
				throw;
			}

			return result;
		}

		public async Task SaveAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken)
		{
			foreach (var @event in events)
			{
				await client.CreateDocumentAsync(documentCollectionUri, @event, null, false, cancellationToken);
			}
		}
	}
}
