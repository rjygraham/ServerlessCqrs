using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ServerlessCqrs.WebJobs.Extensions.Config;
using System;

namespace ServerlessCqrs.WebJobs.Extensions
{
	public static class ServerlessCqrsWebJobsBuilderExtensions
	{
		public static IWebJobsBuilder AddServerlessCqrs(this IWebJobsBuilder builder)
		{
			if (builder == null)
			{
				throw new ArgumentNullException(nameof(builder));
			}

			builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IBindingProvider, CommandContextBindingProvider>());

			builder.AddExtension<ServerlessCqrsExtensionConfigProvider>()
				.BindOptions<ServerlessCqrsOptions>();

			return builder;
		}
	}
}
