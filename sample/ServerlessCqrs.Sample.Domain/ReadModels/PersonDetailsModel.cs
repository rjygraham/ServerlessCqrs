using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ServerlessCqrs.Sample.Domain.ValueTypes;
using System;

namespace ServerlessCqrs.Sample.Domain.ReadModels
{
	[JsonObject(NamingStrategyType = (typeof(CamelCaseNamingStrategy)))]
	public class PersonDetailsModel
	{
		public Guid Id { get; set; }
		public int Version { get; set; }
		public string GivenName { get; set; }
		public string Surname { get; set; }
		public DateTime Dob { get; set; }
		public Gender Gender { get; set; }
	}
}
