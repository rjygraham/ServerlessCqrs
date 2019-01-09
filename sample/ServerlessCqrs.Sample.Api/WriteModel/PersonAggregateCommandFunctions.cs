using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using ServerlessCqrs.Sample.Api.Models;
using ServerlessCqrs.Sample.Domain.Aggregates;
using ServerlessCqrs.WebJobs.Extensions;
using ServerlessCqrs.WebJobs.Extensions.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ServerlessCqrs.Sample.Api.WriteModel
{
	[ServerlessCqrs("cqrs", "events", CosmosDbConnectionStringSetting = "CosmosDbConnectionString", EventGridConnectionStringSetting = "EventGridConnectionString")]
	public static class PersonAggregateCommandFunctions
    {
        [FunctionName("CreatePerson")]
        public static async Task<IActionResult> CreatePersonAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "people")] HttpRequestMessage req,
			[ServerlessCqrsCommandContext] ICommandContext commandContext,
            ILogger log)
        {
			var model = await req.Content.ReadAsAsync<Person>();

			var session = commandContext.GetSession();

			await session.AddAsync(new PersonAggregate(model.Id.Value, model.GivenName, model.Surname, model.Dob.Value, (Domain.ValueTypes.Gender)model.Gender.Value));

			await session.CommitChangesAsync();

			return new CreatedResult(new Uri($"people/{model.Id}", UriKind.Relative), null);
		}

		[FunctionName("UpdatePerson")]
		public static async Task<IActionResult> UpdatePersonAsync(
			[HttpTrigger(AuthorizationLevel.Function, "patch", Route = "people/{id}")] HttpRequestMessage req,
			[ServerlessCqrsCommandContext] ICommandContext boundedContext,
			string id,
			ILogger log)
		{
			var model = await req.Content.ReadAsAsync<Person>();

			var session = boundedContext.GetSession();

			var aggregate = await session.GetAsync<PersonAggregate>(Guid.Parse(id), model.Version);
			aggregate.ChangeName(model.GivenName, model.Surname);

			await session.CommitChangesAsync();

			return new OkResult();
		}

		/// <summary>
		/// This is an extraneous exemple of multiple events emmitted in a single session.
		/// </summary>
		/// <returns></returns>
		[FunctionName("CreateUpdatePerson")]
		public static async Task<IActionResult> CreatUpdatePersonAsync(
		   [HttpTrigger(AuthorizationLevel.Function, "post", Route = "people2")] HttpRequestMessage req,
		   [ServerlessCqrsCommandContext] ICommandContext commandContext,
		   ILogger log)
		{
			var model = await req.Content.ReadAsAsync<Person>();

			var session = commandContext.GetSession();

			var person = new PersonAggregate(model.Id.Value, model.GivenName, model.Surname, model.Dob.Value, (Domain.ValueTypes.Gender)model.Gender.Value);
			person.ChangeName("foo", "bar");

			await session.AddAsync(person);

			await session.CommitChangesAsync();

			return new CreatedResult(new Uri($"people/{model.Id}", UriKind.Relative), null);
		}
	}
}
