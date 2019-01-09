using Microsoft.Azure.WebJobs;
using ServerlessCqrs.Domain.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ServerlessCqrs.WebJobs.Extensions.Utils
{
	internal static class EventTypeHandlerMapping
	{
		private static ConcurrentDictionary<string, HashSet<string>> mappings;

		internal static void InitializedMapping()
		{
			if (mappings == null)
			{
				mappings = new ConcurrentDictionary<string, HashSet<string>>();

				var allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes());

				var events = allTypes.Where(w => typeof(IEvent).IsAssignableFrom(w) && !w.IsInterface && !w.IsAbstract).ToList();
				var potentialEventHandlers = allTypes.SelectMany(s => s.GetMethods().Where(wm => wm.GetCustomAttributes(typeof(FunctionNameAttribute), false).Any() && wm.GetParameters().Where(wp => wp.GetCustomAttributes(typeof(ActivityTriggerAttribute), false).Any()).Any())).ToList();

				foreach (var @event in events)
				{
					var eventHandlers = potentialEventHandlers
						.SelectMany(
							s => s.GetCustomAttributes(typeof(FunctionNameAttribute), false)
							.Cast<FunctionNameAttribute>())
							.Where(w => w.Name.Contains(@event.Name)
						);

					mappings.TryAdd(@event.Name, new HashSet<string>(eventHandlers.Select(s => s.Name)));
				}
			}
		}

		internal static HashSet<string> GetEventHandlers(string eventName)
		{
			mappings.TryGetValue(eventName, out HashSet<string> eventHandlers);
			return eventHandlers;
		}
	}
}
