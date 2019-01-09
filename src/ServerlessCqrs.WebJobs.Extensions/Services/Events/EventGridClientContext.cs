using Microsoft.Azure.EventGrid;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessCqrs.WebJobs.Extensions.Services.Events
{
	internal class EventGridClientContext
	{
		public EventGridClient EventGridClient { get; private set; }
		public string TopicHostName { get; private set; }

		public EventGridClientContext(EventGridClient eventGridClient, string topicHostName)
		{
			EventGridClient = eventGridClient;
			TopicHostName = topicHostName;
		}
	}
}
