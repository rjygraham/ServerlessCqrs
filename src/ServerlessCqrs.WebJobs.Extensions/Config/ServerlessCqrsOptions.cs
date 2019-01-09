using Microsoft.Azure.Documents.Client;

namespace ServerlessCqrs.WebJobs.Extensions.Config
{
	public class ServerlessCqrsOptions
	{
		/// <summary>
		/// Gets or sets the ConnectionMode used in the DocumentClient instances.
		/// </summary>
		public ConnectionMode? ConnectionMode { get; set; }

		/// <summary>
		/// Gets or sets the Protocol used in the DocumentClient instances.
		/// </summary>
		public Protocol? Protocol { get; set; }
	}
}
