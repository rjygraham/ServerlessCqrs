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
	public static class PersonDetailsModelFunctions
	{
		[FunctionName("PersonDetailsModel_PersonCreated")]
		public static void PersonCreated(
		[ActivityTrigger] PersonCreated @event,
		[CosmosDB("cqrs", "people-details",
			ConnectionStringSetting = "CosmosDbConnectionString",
			PartitionKey = "id",
			CollectionThroughput = 400,
			CreateIfNotExists = true)] out dynamic model,
		ILogger log)
		{
			model = new PersonDetailsModel
			{
				Id = @event.AggregateId,
				GivenName = @event.GivenName,
				Surname = @event.Surname,
				Dob = @event.Dob,
				Gender = @event.Gender,
				Version = @event.Version
			};
		}

		[FunctionName("PersonDetailsModel_PersonNameChanged")]
		public static async Task PersonNameChanged([ActivityTrigger] PersonNameChanged @event,
		[CosmosDB("cqrs", "people-details",
			ConnectionStringSetting = "CosmosDbConnectionString",
			PartitionKey = "id",
			CollectionThroughput = 400,
			CreateIfNotExists = true)] DocumentClient client,
		ILogger log)
		{
			var requestOptions = new RequestOptions { PartitionKey = new PartitionKey(@event.AggregateId.ToString()) };

			var response = await client.ReadDocumentAsync<PersonDetailsModel>(UriFactory.CreateDocumentUri("cqrs", "people-details", @event.AggregateId.ToString()), requestOptions);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				if (response.Document.Version > (@event.Version - 1))
				{
					throw new Exception("Events out of order.");
				}

				var personDetails = response.Document;
				personDetails.GivenName = @event.NewGivenName;
				personDetails.Surname = @event.NewSurname;
				personDetails.Version = @event.Version;

				await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri("cqrs", "people-details", personDetails.Id.ToString()), personDetails, requestOptions);
			}

			log.LogInformation(@event.ToString());
		}
	}
}
