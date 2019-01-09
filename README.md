Serverless CQRS Extension for Azure Functions
===
This repository contains code of an experimental Event Sourcing/Command Query Responsibility Segregation extension for [Azure Functions](https://github.com/Azure/azure-webjobs-sdk) that I'm using in a moonlighting project. I've included brief sections about CQRS and ES below with links to relevant learning material.

From an ES perspective this extension is very prescriptive in that it requires Azure Functions for the runtime, Cosmos DB for the event store, and Azure Event Grid for publishing events. However, outside of usage of Durable Functions for orchestrating the application of events to readmodels, the readmodel side has been left intentionally non-prescriptive to provide flexibility in choosing the appropriate data store for each model.

The core `AggregateRoot` and some eventing logic is a derivative of the simple yet awesome CQRS/ES framework CQRSLite found here: [https://github.com/gautema/CQRSlite/](https://github.com/gautema/CQRSlite/). The Apache 2 license has been carried forward for all in scope source.

**NOTE: This extension is strictly experimental at this time. It's an early proof of concept and has not been used in production, does not have any automated testing, and does not implement any kind rety/compensation logic. Use at your peril.**

# Getting Started
To get started using ServerlessCqrs, we recommend cloning or downloading the zip of this repo and exploring the sample API application included. Please note, the following prerequisites are required.

## Prerequitesites
- Visual Studio 2017 (Community or above): [Develop Azure Functions using Visual Studio](https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs)
- OR Azure Functions Core Tools and .NET Core 2.1: [Work with Azure Functions Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local)
- [Azure CLI 2.0](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest) (optional for Cosmos DB setup steps below)
- [Azure Storage Emulator](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator)
- [Azure Cosmos DB Emulator](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator)

## Setup Cosmos DB
Once you've installed the prerequisites above the next step is to create the required collections in the Cosmos DB Emulator. The easiest way to accomplish this is to execute the commands immediately below via the Azure CLI.

Alternatively, you can create the database and document collections using the Emulator's UI. The database name and collection names/partition keys must be an exact match to the values in the commands; otherwise, you'll need to modify the sample code to match the values you use.

```bash
az cosmosdb database create ^
	--db-name "cqrs" ^
 	--key "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==" ^
	--url-connection "https://localhost:8081/"

az cosmosdb collection create ^
	--db-name "cqrs" ^
	--collection-name "events" ^
	--partition-key-path "/partitionKey" ^
	--throughput 400 ^
	--key "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==" ^
	--url-connection "https://localhost:8081/"

az cosmosdb collection create ^
	--db-name "cqrs" ^
	--collection-name "lists" ^
	--partition-key-path "/partitionKey" ^
	--throughput 400 ^
	--key "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==" ^
	--url-connection "https://localhost:8081/"

az cosmosdb collection create ^
	--db-name "cqrs" ^
	--collection-name "people-details" ^
	--partition-key-path "/id" ^
	--throughput 400 ^
	--key "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==" ^
	--url-connection "https://localhost:8081/"
````
## Azure

For those of you that'd prefer to utilize Azure for testing the sample app, I'll be adding an Azure Resource Manager template to deploy a Cosmos DB, Event Grid, and Function App in the near future.

# Exploring The Sample

With the prerequisites installed and the Cosmos DB collections created, you can start exploring the sample app. Do this by opening the ServerlessCqrs solution with Visual Studio and starting the `ServerlessCqrs.Sample.Api` in the Sample folder. Alternatively, you can open up a shell, change directories to `sample/ServerlessCqrs.Sample.Api`, and execute this command:

```bash
func start --build
````

We'll use cURL to exercise the sample app API. I'd encourage you to explorer the Cosmos DB collections using the emulator's Data Explorer as you exeucte each step.

*NOTE: I will say this about the extension and sample - CQRS/ES purists may very well hate it. I made a design decision to forego the typical Command Sender/Command Handler part of the pattern/architecture since the Function HttpTrigger is in effect a Command Handler and the calling app the Command Sender. Furthermore, the sample abides by REST which is somewhat unnatural since CQRS aligns more closely with an RPC-style API surface area.*

***Feedback is welcomed.***

## Create a Person

To create a person we'll POST to our `/api/people` REST endpoint. 

```bash
curl -X POST -H "Content-Type: application/json" ^
-d "{ \"id\": \"9a9fcb57-352d-411c-880b-76b9f5a62ca4\", \"givenName\": \"John\", \"surname\": \"Doe\", \"dob\": \"2001-01-01\", \"gender\": \"male\" }" ^
http://localhost:7071/api/people
````

You should receive a 201 Created response with a `Location: people/9a9fcb57-351d-411c-880b-76b9f5a62ca4` header.

So what happened? Well, when you POSTed to the `/api/people` endpoint, you triggered the following sequence:

1. Within the function, the model was read from the POST body
1. A new `PersonAggregate` was instantiated using the values contained in the model, the aggregate instance was added to an ICommandContext session, and internally a `PersonCreated` event was created.
1. The session changes were committed which caused the `PersonCreated` event to be saved in the Cosmos DB `events` document collection. The `PersonCreated` event was also emitted to the readmodel Event Handlers responsible for keeping the readmodel updated.

## Get Specific Person

Use the `Location` header value returned from the POST to issue a GET to retrieve the person we just created.

```bash
curl -X GET -H "Accept: application/json" ^
http://localhost:7071/api/people/9a9fcb57-352d-411c-880b-76b9f5a62ca4
````
During the POST above, an Event Handler received the `PersonCreated` event and materialized the `PersonDetailsModel` which we just queried for directly by ID.

## Update Person's Name

Now update the person's name from John to Joe by issueing the following PATCH command.

```bash
curl -X PATCH -H "Content-Type: application/json" ^
-d "{ \"givenName\": \"Joe\", \"surname\": \"Doe\" }" ^
http://localhost:7071/api/people/9a9fcb57-352d-411c-880b-76b9f5a62ca4
````
Sending the PATCH request just caused a `PersonNameChanged` event to be recorded in the `events` collection and emitted and handled by the appropriate Event Handlers to update the readmodels.

## Get All People

Finally, get all people and observe that John's name is now Joe.

```bash
curl -X GET -H "Accept: application/json" http://localhost:7071/api/people
````

## Experiment

Experiment with the API by issuing new POST, PATCH, and GET requests to the various endpoints. Try creating a new PersonAggregate via the POST call, but reuse the same GUID value for the ID.

# Resources

In the event it might be helpful, here are some links to useful resources about the CQRS and Event Sourcing patterns.

## Command Query Responsibility Segregation

Command Query Responsibility Segregation (CQRS) is an design pattern in which a system segregates its reads from its writes using a different model for each. When applied to the appropriate problems and implemented correctly, it affords a great deal of flexibility and scabability. You can read more about CQRS at the links below:

- [CQRS, Task Based UIs, Event Sourcing agh!](http://codebetter.com/gregyoung/2010/02/16/cqrs-task-based-uis-event-sourcing-agh/)
- [CQRS/DDD by Greg Young](https://www.youtube.com/watch?v=KXqrBySgX-s)
- [Martin Fowler: CQRS](https://www.martinfowler.com/bliki/CQRS.html)

## Event Sourcing

Although it's not required, CQRS is often employed with another pattern called Event Sourcing (ES). Event Sourcing stores an object's state as a series of events rather than a single representation of current state. As Martin Fowler put's it, with Event Sourcing you don't just see where you are, but also how you got there.

- [Microsoft Docs: Event Sourcing pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/event-sourcing)
- [Greg Young - CQRS and Event Sourcing - Code on the Beach 2014](https://www.youtube.com/watch?v=JHGkaShoyNs)
- [Martin Fowler: Event Sourcing](https://martinfowler.com/eaaDev/EventSourcing.html)


# License

Except where denoted, this project is licensed under the [MIT License](LICENSE.md)