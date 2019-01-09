using Microsoft.Azure.WebJobs;
using System.Net.Http;
using System.Threading.Tasks;

namespace ServerlessCqrs.WebJobs.Extensions.Services.Events
{
	public interface IEventWebhookHandler
	{
		Task<HttpResponseMessage> HandleAsync(HttpRequestMessage request, DurableOrchestrationClient durableClient);
	}
}
