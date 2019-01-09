using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using ServerlessCqrs.Sample.Domain.ReadModels;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace ServerlessCqrs.Sample.Api.ReadModel
{
	public static class PersonAggregateQueryFunctions
    {
        [FunctionName("GetAllPeople")]
        public static HttpResponseMessage GetAllPeople(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "people")] HttpRequestMessage req,
			[CosmosDB("cqrs", "lists",
				ConnectionStringSetting = "CosmosDbConnectionString",
				PartitionKey = "PersonListModel",
				SqlQuery = "SELECT l.id, l.fullName FROM l WHERE l.partitionKey = 'PersonListModel'",
				CollectionThroughput = 400,
				CreateIfNotExists = true)] IEnumerable<PersonListModel> people,
            ILogger log
		)
        {
			return req.CreateResponse(HttpStatusCode.OK, people);
        }

		[FunctionName("GetPerson")]
		public static HttpResponseMessage GetPersonAsync(
			[HttpTrigger(AuthorizationLevel.Function, "get", Route = "people/{id}")] HttpRequestMessage req,
			string id,
			[CosmosDB("cqrs", "people-details",
				ConnectionStringSetting = "CosmosDbConnectionString", Id ="{id}", PartitionKey = "{id}", CollectionThroughput = 400, CreateIfNotExists = true)] PersonDetailsModel model,
			ILogger log
		)
		{
			return model != null
				? req.CreateResponse(HttpStatusCode.OK, model)
				: req.CreateResponse(HttpStatusCode.NotFound);
		}
	}
}
