using Microsoft.Azure.WebJobs.Host.Bindings;
using ServerlessCqrs.WebJobs.Extensions.Services;
using System;
using System.Threading.Tasks;

namespace ServerlessCqrs.WebJobs.Extensions.Config
{
	internal class CommandContextValueProvider : IValueProvider
	{
		private readonly CommandContextDescriptor descriptor;
		private readonly ServerlessCqrsOptions options;

		public Type Type => typeof(ICommandContext);

		internal CommandContextValueProvider(CommandContextDescriptor descriptor, ServerlessCqrsOptions options)
		{
			this.descriptor = descriptor;
			this.options = options;
		}

		public Task<object> GetValueAsync()
		{
			return Task.FromResult<object>(new CommandContext(descriptor, options));
		}

		public string ToInvokeString()
		{
			return null;
		}
	}
}
