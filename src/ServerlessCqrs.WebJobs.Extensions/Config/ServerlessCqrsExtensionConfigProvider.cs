using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServerlessCqrs.WebJobs.Extensions.Bindings;
using ServerlessCqrs.WebJobs.Extensions.Services.Events;
using System;

namespace ServerlessCqrs.WebJobs.Extensions.Config
{
	/// <summary>
	/// Defines the configuration options for the ServerlessCqrs bindings.
	/// </summary>
	[Extension("ServerlessCqrs")]
	internal class ServerlessCqrsExtensionConfigProvider : IExtensionConfigProvider
	{
		private readonly ILoggerFactory loggerFactory;

		public ServerlessCqrsExtensionConfigProvider(ILoggerFactory loggerFactory)
		{
			this.loggerFactory = loggerFactory;
		}

		public void Initialize(ExtensionConfigContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}

			var eventWebhookHandlerRule = context.AddBindingRule<ServerlessCqrsEventWebhookHandlerAttribute>();
			eventWebhookHandlerRule.BindToInput(new EventWebhookHandlerBuilder());

			var eventOrchestrationHandlerRule = context.AddBindingRule<ServerlessCqrsEventOrchestrationHandlerAttribute>();
			eventOrchestrationHandlerRule.BindToInput(new EventOrchestrationHandlerBuilder());
		}
	}
}
