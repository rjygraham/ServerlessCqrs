using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Options;
using ServerlessCqrs.WebJobs.Extensions.Services;
using System.Reflection;
using System.Threading.Tasks;

namespace ServerlessCqrs.WebJobs.Extensions.Config
{
	internal class CommandContextBindingProvider : IBindingProvider
	{
		private readonly INameResolver nameResolver;
		private readonly ServerlessCqrsOptions options;

		public CommandContextBindingProvider(INameResolver nameResolver, IOptions<ServerlessCqrsOptions> options)
		{
			this.nameResolver = nameResolver;
			this.options = options?.Value;
		}

		public Task<IBinding> TryCreateAsync(BindingProviderContext context)
		{
			ParameterInfo parameter = context.Parameter;

			if (parameter.ParameterType != typeof(ICommandContext))
			{
				return Task.FromResult<IBinding>(null);
			}

			/// For Functions demarked with ServerlessCqrsCommandContextAttribute, check the declaring type
			/// for presence of ServerlessCqrsAttribute and capture any global settings from it.
			var parameterAttribute = parameter.GetCustomAttribute<ServerlessCqrsCommandContextAttribute>();
			var typeAttribute = parameter.Member.DeclaringType.GetCustomAttribute<ServerlessCqrsAttribute>(false);
	
			var descriptor = CommandContextDescriptor.Create(nameResolver, parameterAttribute, typeAttribute ?? new ServerlessCqrsAttribute());

			return Task.FromResult<IBinding>(new CommandContextBinding(parameter.Name, descriptor, options));
		}
	}
}
