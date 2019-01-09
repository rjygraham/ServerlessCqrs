using Microsoft.Azure.WebJobs;
using ServerlessCqrs.WebJobs.Extensions.Services.Events;

namespace ServerlessCqrs.WebJobs.Extensions.Bindings
{
	internal class EventOrchestrationHandlerBuilder : IConverter<ServerlessCqrsEventOrchestrationHandlerAttribute, IEventOrchestrationHandler>
	{
		public IEventOrchestrationHandler Convert(ServerlessCqrsEventOrchestrationHandlerAttribute input)
		{
			return new EventOrchestrationHandler();
		}
	}
}
