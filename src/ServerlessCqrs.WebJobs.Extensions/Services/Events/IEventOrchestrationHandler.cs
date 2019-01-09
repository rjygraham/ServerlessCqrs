using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerlessCqrs.WebJobs.Extensions.Services.Events
{
	public interface IEventOrchestrationHandler
	{
		Task<bool> HandleAsync(DurableOrchestrationContext context, CancellationToken cancellationToken = default);
	}
}
