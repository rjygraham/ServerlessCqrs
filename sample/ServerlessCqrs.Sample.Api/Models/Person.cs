using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessCqrs.Sample.Api.Models
{
	[JsonObject(ItemNullValueHandling =	NullValueHandling.Ignore)]
	public class Person
	{
		public Guid? Id { get; set; }
		public string GivenName { get; set; }
		public string Surname { get; set; }
		public DateTime? Dob { get; set; }
		public Gender? Gender { get; set; }
		public int? Version { get; set; }
	}
}
