using Microsoft.Azure.WebJobs.Description;
using System;

namespace ServerlessCqrs.WebJobs.Extensions
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	[Binding]
	public sealed class ServerlessCqrsEventOrchestrationHandlerAttribute : Attribute
	{
	}
}
