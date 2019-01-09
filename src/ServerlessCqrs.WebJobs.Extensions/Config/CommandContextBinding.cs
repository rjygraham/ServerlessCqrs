using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using System.Threading.Tasks;

namespace ServerlessCqrs.WebJobs.Extensions.Config
{
	internal class CommandContextBinding : IBinding
	{
		private readonly string parameterName;
		private readonly ServerlessCqrsOptions options;
		private readonly CommandContextDescriptor descriptor;

		public bool FromAttribute => false;

		internal CommandContextBinding(string parameterName, CommandContextDescriptor descriptor, ServerlessCqrsOptions options)
		{
			this.parameterName = parameterName;
			this.descriptor = descriptor;
			this.options = options;
		}

		public Task<IValueProvider> BindAsync(object value, ValueBindingContext context)
		{
			return BindAsync();
		}

		public Task<IValueProvider> BindAsync(BindingContext context)
		{
			return BindAsync();
		}

		private Task<IValueProvider> BindAsync()
		{
			IValueProvider result = new CommandContextValueProvider(descriptor, options);
			return Task.FromResult(result);
		}

		public ParameterDescriptor ToParameterDescriptor()
		{
			return new ParameterDescriptor { Name = parameterName };
		}
	}
}
