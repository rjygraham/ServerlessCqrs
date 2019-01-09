using ServerlessCqrs.WebJobs.Extensions.Config;
using ServerlessCqrs.WebJobs.Extensions.Services.Events;

namespace ServerlessCqrs.WebJobs.Extensions.Services
{
	internal class CommandContext : ICommandContext
	{
		private readonly ServerlessCqrsOptions options;
		private readonly CommandContextDescriptor descriptor;

		internal CommandContext(CommandContextDescriptor descriptor, ServerlessCqrsOptions options)
		{
			this.descriptor = descriptor;
			this.options = options;
		}

		public ISession GetSession()
		{
			var documentClient = DocumentClientFactory.GetDocumentClientInstance(descriptor.CosmosDbConnectionString, options);
			var cosmosDbEventStore = new CosmosDbEventStore(documentClient, descriptor.DatabaseName, descriptor.CollectionName);

			var eventGridClientContext = EventGridClientFactory.GetEventGridClientContext(descriptor.EventGridConnectionString);
			var eventGridPublisher = new EventGridEventPublisher(eventGridClientContext.EventGridClient, eventGridClientContext.TopicHostName);

			return new Session(new Repository(cosmosDbEventStore, eventGridPublisher));
		}
	}
}
