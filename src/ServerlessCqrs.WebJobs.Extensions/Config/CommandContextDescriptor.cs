using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessCqrs.WebJobs.Extensions.Config
{
	internal class CommandContextDescriptor
	{
		public string CosmosDbConnectionString { get; }
		public string DatabaseName { get; }
		public string CollectionName { get; }
		public string EventGridConnectionString { get; }

		private CommandContextDescriptor(string cosmosDbConnectionString, string databaseName, string collectionName, string eventGridConnectionString)
		{
			CosmosDbConnectionString = cosmosDbConnectionString ?? throw new ArgumentNullException(nameof(cosmosDbConnectionString));
			DatabaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
			CollectionName = collectionName ?? throw new ArgumentNullException(nameof(collectionName));
			EventGridConnectionString = eventGridConnectionString ?? throw new ArgumentNullException(nameof(eventGridConnectionString));
		}

		internal static CommandContextDescriptor Create(INameResolver nameResolver, ServerlessCqrsCommandContextAttribute parameterAttribute, ServerlessCqrsAttribute typeAttribute)
		{
			/// Values from ServerlessCqrsCommandContextAttribute take precedence over ServerlessCqrsAttribute.
			/// Also note that AutoResolve and AppSettings values don't hydrate here (at least 
			/// I haven't figured that out yet).
			var cosmosDbConnectionString = !string.IsNullOrWhiteSpace(parameterAttribute.CosmosDbConnectionStringSetting)
				? nameResolver.Resolve(parameterAttribute.CosmosDbConnectionStringSetting)
				: nameResolver.Resolve(typeAttribute.CosmosDbConnectionStringSetting);

			var eventGridConnectionString = !string.IsNullOrWhiteSpace(parameterAttribute.EventGridConnectionStringSetting)
				? nameResolver.Resolve(parameterAttribute.EventGridConnectionStringSetting)
				: nameResolver.Resolve(typeAttribute.EventGridConnectionStringSetting);

			string databaseName = parameterAttribute.DatabaseName;
			string collectionName = parameterAttribute.CollectionName;

			if (string.IsNullOrWhiteSpace(databaseName) || string.IsNullOrWhiteSpace(collectionName))
			{
				databaseName = typeAttribute.DatabaseName;
				collectionName = typeAttribute.CollectionName;
			}

			return new CommandContextDescriptor(cosmosDbConnectionString, databaseName, collectionName, eventGridConnectionString);
		}

	}
}
