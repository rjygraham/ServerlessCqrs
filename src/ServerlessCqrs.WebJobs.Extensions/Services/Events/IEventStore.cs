// Copyright 2018 Gaute Magnussen

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//	http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// NOTE: Portions of this file may have been modified from their original
// source found here: https://github.com/gautema/CQRSlite/

using ServerlessCqrs.Domain.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ServerlessCqrs.WebJobs.Extensions.Services.Events
{
	public interface IEventStore
	{
		Task SaveAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default);
		Task<IEnumerable<IEvent>> GetAsync(Guid aggregateId, int fromVersion, string partitionKey, CancellationToken cancellationToken = default);
	}
}
