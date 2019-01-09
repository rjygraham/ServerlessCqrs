using Newtonsoft.Json;

namespace ServerlessCqrs.WebJobs.Extensions
{
	public static class ServerlessCqrsConstants
	{
		internal const string DefaultDatabaseName = "cqrs";
		internal const string DefaultCollectionName = "events";
		internal const string AccountEndpoint = "AccountEndpoint";
		internal const string AccountKey = "AccountKey";
		internal const string EventGridValidationEventType = "Microsoft.EventGrid.SubscriptionValidationEvent";

		internal static JsonSerializerSettings AutoJsonSerializerSettings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.Auto
		};
	}
}
