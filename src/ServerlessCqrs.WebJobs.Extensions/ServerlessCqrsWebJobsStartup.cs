using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using ServerlessCqrs.WebJobs.Extensions;
using ServerlessCqrs.WebJobs.Extensions.Config;
using ServerlessCqrs.WebJobs.Extensions.Utils;

[assembly: WebJobsStartup(typeof(ServerlessCqrsWebJobsStartup))]

namespace ServerlessCqrs.WebJobs.Extensions
{
	public class ServerlessCqrsWebJobsStartup : IWebJobsStartup
	{
		public void Configure(IWebJobsBuilder builder)
		{
			builder.AddServerlessCqrs();
		}
	}
}
