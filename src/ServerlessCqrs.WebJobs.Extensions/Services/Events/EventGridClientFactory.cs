using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Newtonsoft.Json;
using ServerlessCqrs.WebJobs.Extensions.Config;
using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ServerlessCqrs.WebJobs.Extensions.Services.Events
{
	public static class EventGridClientFactory
	{
		private static ConcurrentDictionary<string, EventGridClientContext> instances = new ConcurrentDictionary<string, EventGridClientContext>();
		
		internal static EventGridClientContext GetEventGridClientContext(string connectionString)
		{
			if (!instances.TryGetValue(connectionString, out EventGridClientContext result))
			{
				var connectionDetails = ConnectionStringHelper.ParseConnectionString(connectionString);
				var credentials = new TopicCredentials(connectionDetails.Key);

				// If we're working locally add in delegating handler to change scheme to http.
				// This is just to make it easier... so we don't have to setup ngrok and an Event Grid Topic.
				var client = connectionDetails.Endpoint.Contains("localhost")
					? new EventGridClient(credentials, new LocalhostHandler())
					: new EventGridClient(credentials);

				client.SerializationSettings.TypeNameHandling = TypeNameHandling.Auto;
			
				result = new EventGridClientContext(client, connectionDetails.Endpoint);

				instances.TryAdd(connectionString, result);
			}

			return result;
		}

		internal class LocalhostHandler : DelegatingHandler
		{
			protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
			{
				if (request.RequestUri.Host.Equals("localhost"))
				{
					request.RequestUri = new Uri($"http://{request.RequestUri.Authority}{request.RequestUri.PathAndQuery}");
				}

				return base.SendAsync(request, cancellationToken);
			}
		}
	}
}
