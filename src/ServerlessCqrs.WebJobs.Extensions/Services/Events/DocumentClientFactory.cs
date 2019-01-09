using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ServerlessCqrs.WebJobs.Extensions.Config;
using System;
using System.Collections.Concurrent;

namespace ServerlessCqrs.WebJobs.Extensions.Services.Events
{
	internal static class DocumentClientFactory
	{
		private static ConcurrentDictionary<string, DocumentClient> instances = new ConcurrentDictionary<string, DocumentClient>();

		internal static DocumentClient GetDocumentClientInstance(string connectionString, ServerlessCqrsOptions options)
		{
			if (!instances.TryGetValue(connectionString, out DocumentClient result))
			{
				var connectionDetails = ConnectionStringHelper.ParseConnectionString(connectionString);
				var connectionPolicy = GetConnectionPolicy(options);
				var serializerSettings = GetSerializerSettings();
				result = new DocumentClient(new Uri(connectionDetails.Endpoint), connectionDetails.Key, serializerSettings, connectionPolicy);

				instances.TryAdd(connectionString, result);
			}

			return result;
		}

		private static ConnectionPolicy GetConnectionPolicy(ServerlessCqrsOptions options)
		{
			var result = new ConnectionPolicy();

			if (options.ConnectionMode.HasValue)
			{
				result.ConnectionMode = options.ConnectionMode.Value;
			}

			if (options.Protocol.HasValue)
			{
				result.ConnectionProtocol = options.Protocol.Value;
			}

			return result;
		}

		private static JsonSerializerSettings GetSerializerSettings()
		{
			return new JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver(),
				TypeNameHandling = TypeNameHandling.Objects
			};
		}
	}
}
