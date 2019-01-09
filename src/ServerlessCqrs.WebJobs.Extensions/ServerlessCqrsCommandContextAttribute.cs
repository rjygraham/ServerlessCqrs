using Microsoft.Azure.WebJobs.Description;
using System;

namespace ServerlessCqrs.WebJobs.Extensions
{
	/// <summary>
	/// Attribute used to obtain reference to an ICommandContext as Function input parameter.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	[Binding]
	public sealed class ServerlessCqrsCommandContextAttribute : Attribute
	{
		[AppSetting]
		public string CosmosDbConnectionStringSetting { get; set; }

		[AutoResolve]
		public string DatabaseName { get; private set; }

		[AutoResolve]
		public string CollectionName { get; private set; }

		[AppSetting]
		public string EventGridConnectionStringSetting { get; set; }

		/// <remarks>
		/// When using the parameterless constuctor, you must apply the ServerlessCqrsAttribute to the containing class.
		/// </remarks>
		public ServerlessCqrsCommandContextAttribute()
		{
		}

		public ServerlessCqrsCommandContextAttribute(string databaseName, string collectionName)
		{
			DatabaseName = databaseName;
			CollectionName = collectionName;
		}
	}
}
