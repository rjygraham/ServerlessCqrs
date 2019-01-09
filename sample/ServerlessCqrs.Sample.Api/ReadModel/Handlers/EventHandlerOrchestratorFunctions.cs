using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using ServerlessCqrs.WebJobs.Extensions;
using ServerlessCqrs.WebJobs.Extensions.Services.Events;
using System.Net.Http;
using System.Threading.Tasks;

namespace ServerlessCqrs.Sample.Api.ReadModel.Handlers
{
	public static class EventHandlerOrchestratorFunctions
	{
		[FunctionName("HandleEventWebhook")]
		public static async Task<HttpResponseMessage> HandleEventWebhookAsync(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "events")] HttpRequestMessage request,
			[OrchestrationClient]DurableOrchestrationClient durableClient,
			[ServerlessCqrsEventWebhookHandler("EventHandlerOrchestrator")] IEventWebhookHandler handler
		)
		{
			return await handler.HandleAsync(request, durableClient);
		}

		[FunctionName("EventHandlerOrchestrator")]
		public static async Task<bool> HandleEventOrchestratorAsync(
			[OrchestrationTrigger] DurableOrchestrationContext context,
			[ServerlessCqrsEventOrchestrationHandler] IEventOrchestrationHandler handler)
		{
			return await handler.HandleAsync(context);
		}
	}
}
