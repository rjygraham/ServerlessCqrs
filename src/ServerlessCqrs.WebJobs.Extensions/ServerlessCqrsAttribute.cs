using Microsoft.Azure.WebJobs.Description;
using System;

namespace ServerlessCqrs.WebJobs.Extensions
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	[Binding]
	public class ServerlessCqrsAttribute : Attribute
	{
		public string CosmosDbConnectionStringSetting { get; set; }

		public string DatabaseName { get; private set; }

		public string CollectionName { get; private set; }

		public string EventGridConnectionStringSetting { get; set; }

		public ServerlessCqrsAttribute()
		{
		}

		public ServerlessCqrsAttribute(string databaseName, string collectionName)
		{
			DatabaseName = databaseName;
			CollectionName = collectionName;
		}
	}
}
