using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ServerlessCqrs.Sample.Domain.Events;
using ServerlessCqrs.Sample.Domain.ReadModels;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ServerlessCqrs.Sample.Api.ReadModel.Handlers
{
	public static class PersonListModelFunctions
    {
		private static string PartitionKeyValue = nameof(PersonListModel);

		[FunctionName("PersonListModel_PersonCreated")]
		public static void PersonCreated(
		 [ActivityTrigger] PersonCreated @event,
		 [CosmosDB("cqrs", "lists",
			ConnectionStringSetting = "CosmosDbConnectionString",
			PartitionKey = "partitionKey",
			CollectionThroughput = 400,
			CreateIfNotExists = true)] out dynamic model,
		 ILogger log)
		{
			model = new PersonListModel
			{
				Id = @event.AggregateId,
				FullName = $"{@event.Surname}, {@event.GivenName}",
				Version = @event.Version,
				PartitionKey = PartitionKeyValue
			};
		}

		[FunctionName("PersonListModel_PersonNameChanged")]
		public static async Task PersonNameChanged([ActivityTrigger] PersonNameChanged @event,
		[CosmosDB("cqrs", "lists",
			ConnectionStringSetting = "CosmosDbConnectionString",
			PartitionKey = "partitionKey",
			CollectionThroughput = 400,
			CreateIfNotExists = true)] DocumentClient client,
		ILogger log)
		{
			var requestOptions = new RequestOptions { PartitionKey = new PartitionKey(PartitionKeyValue) };

			var response = await client.ReadDocumentAsync<PersonListModel>(UriFactory.CreateDocumentUri("cqrs", "lists", @event.AggregateId.ToString()), requestOptions);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				if (response.Document.Version > (@event.Version - 1))
				{
					throw new Exception("Events out of order.");
				}

				var personList = response.Document;
				personList.FullName = $"{@event.NewSurname}, {@event.NewGivenName}";
				personList.Version = @event.Version;

				await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri("cqrs", "lists", personList.Id.ToString()), personList, requestOptions);
			}

			log.LogInformation(@event.ToString());
		}
	}
}
