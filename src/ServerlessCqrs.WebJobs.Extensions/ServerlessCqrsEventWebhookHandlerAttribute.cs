using Microsoft.Azure.WebJobs.Description;
using System;

namespace ServerlessCqrs.WebJobs.Extensions
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	[Binding]
	public sealed class ServerlessCqrsEventWebhookHandlerAttribute : Attribute
	{
		public string EventHandlerOrchestratorName { get; private set; }

		public ServerlessCqrsEventWebhookHandlerAttribute(string eventHandlerOrchestratorName)
		{
			EventHandlerOrchestratorName = eventHandlerOrchestratorName;
		}
	}
}
