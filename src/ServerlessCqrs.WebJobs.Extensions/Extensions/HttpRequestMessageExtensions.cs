using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace ServerlessCqrs.WebJobs.Extensions.Extensions
{
	public static class HttpRequestMessageExtensions
	{
		public static HttpResponseMessage CreateResponse<T>(this HttpRequestMessage request, T content)
		{
			return new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json")
			};
		}
	}
}
