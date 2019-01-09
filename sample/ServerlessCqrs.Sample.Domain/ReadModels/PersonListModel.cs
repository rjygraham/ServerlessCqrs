using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace ServerlessCqrs.Sample.Domain.ReadModels
{
	[JsonObject(NamingStrategyType = (typeof(CamelCaseNamingStrategy)), ItemNullValueHandling = NullValueHandling.Ignore)]
	public class PersonListModel
	{
		public Guid Id { get; set; }
		public int? Version { get; set; }
		public string FullName { get; set; }
		public string PartitionKey { get; set; }
	}
}
