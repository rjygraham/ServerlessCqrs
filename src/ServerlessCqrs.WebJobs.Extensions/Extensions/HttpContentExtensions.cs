using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCqrs.WebJobs.Extensions.Extensions
{
	public static class HttpContentExtensions
	{
		public static async Task<T> ReadAsAsync<T>(this HttpContent content)
		{
			var json = await content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<T>(json, ServerlessCqrsConstants.AutoJsonSerializerSettings);
		}
	}
}
