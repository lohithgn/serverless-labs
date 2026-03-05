# serverless-labs

A progressive, hands-on lab series for building serverless applications on Azure using **Azure Functions** and **Azure Cosmos DB**. The same scenarios are implemented across multiple language stacks so you can learn in the technology you prefer.

## Choose Your Stack

| Stack | Folder | Status |
|-------|--------|--------|
| **C#** (.NET 10) | [`csharp/`](csharp/) | Available |
| **TypeScript** | `typescript/` | Coming soon |
| **JavaScript** | `javascript/` | Coming soon |
| **Java** | `java/` | Coming soon |

Each folder is **self-contained** with its own labs, infrastructure (Bicep), and `azure.yaml` for Azure Developer CLI (`azd`) deployment. Pick a stack, `cd` into it, and follow the README there.

## What You'll Build

Across the labs you will progressively build a serverless inventory API:

| Lab | Focus | What You Learn |
|-----|-------|----------------|
| **Lab 1** | Basic Functions | HTTP triggers, request validation, in-memory storage, Swagger UI |
| **Lab 2** | Database Integration | CRUD with Azure Cosmos DB, optimistic concurrency |
| **Lab 3** | Event-Driven | Change Feed for real-time processing and alerts |
| **Lab 4** | Orchestration | Durable Functions workflows |

## Prerequisites

- **Visual Studio Code** with the Azure Functions extension
- **Azure Functions Core Tools** (v4.x)
- Language-specific SDK (see each stack's README)
- **Azure CLI** (`az`) and **Azure Developer CLI** (`azd`) — for Labs 2+
- **Azure subscription** with appropriate permissions

## Quick Start

```bash
git clone https://github.com/Azure-Samples/serverless-labs.git
cd serverless-labs/<stack>   # e.g. cd serverless-labs/csharp
# follow the README in that folder
```

## License

This project is licensed under the [MIT License](LICENSE).
