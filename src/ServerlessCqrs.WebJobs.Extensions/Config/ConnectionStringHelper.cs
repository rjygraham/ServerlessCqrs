using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace ServerlessCqrs.WebJobs.Extensions.Config
{
	internal static class ConnectionStringHelper
	{
		internal static (string Endpoint, string Key) ParseConnectionString(string connectionString)
		{
			string endpoint = null;
			string key = null;

			DbConnectionStringBuilder builder = new DbConnectionStringBuilder
			{
				ConnectionString = connectionString
			};

			if (builder.TryGetValue(ServerlessCqrsConstants.AccountKey, out object authKey))
			{
				key = authKey.ToString();
			}

			if (builder.TryGetValue(ServerlessCqrsConstants.AccountEndpoint, out object accountEndpoint))
			{
				endpoint = accountEndpoint.ToString();
			}

			return (endpoint, key);
		}
	}
}
