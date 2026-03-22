# Lab 1: Basic Secure Function with Validation

## Overview

This lab demonstrates creating a simple HTTP-triggered C# Azure Function with proper request validation using [Data Annotations](https://learn.microsoft.com/dotnet/api/system.componentmodel.dataannotations). The focus is on clean, idiomatic C# code following Azure Functions best practices without over-engineering.

## Prerequisites

Before running this lab, ensure you have the following installed:

- **Visual Studio Code** with the [Azure Functions extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions) and the [C# Dev Kit extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
- **Azure Functions Core Tools** (v4.x)
- **.NET 10 SDK** or later
- **Azurite** (local storage emulator) or an Azure Storage account connection string

## Project Structure

```text
lab1/
└── funcProject/
    ├── Program.cs              # Host builder and DI configuration
    ├── HelloEndpoints.cs       # Hello GET/POST endpoints
    ├── HelloRequest.cs         # Request model for HelloUser
    ├── AssetEndpoints.cs       # Asset CRUD endpoints
    ├── AssetStore.cs           # In-memory asset store
    ├── DocsEndpoints.cs        # Custom Swagger UI endpoint
    ├── MyHttpFunction.cs       # Sample HTTP trigger
    ├── Models/
    │   ├── AssetCreate.cs      # Asset creation model with validation
    │   ├── AssetResponse.cs    # Asset response model
    │   ├── AssetList.cs        # Asset list wrapper
    │   ├── AssetStatus.cs      # Asset status enum
    │   └── AssetType.cs        # Asset type enum
    ├── funcProject.csproj      # Project file and dependencies
    ├── host.json               # Azure Functions host configuration
    ├── local.settings.json     # Local development settings
    └── README.md               # This file
```

## Features

- **IT Asset Tracking**: Register, get, and list IT assets with validation
- **Request Validation**: Using [Data Annotations](https://learn.microsoft.com/dotnet/api/system.componentmodel.dataannotations) for model validation
- **Health Check**: Simple hello endpoint
- **Error Handling**: Proper HTTP status codes and error messages
- **Swagger UI**: Custom `/api/docs` endpoint with inline OpenAPI spec
- **Dependency Injection**: Constructor-injected services via the built-in DI container
- **Logging**: Built-in Azure Functions logging with Application Insights

## Running Locally

1. Ensure Azurite is running (or set a valid `AzureWebJobsStorage` connection string in `local.settings.json`)
2. In VS Code, open the Debug view and select the appropriate launch configuration
3. Start debugging — the project will build and start the Functions host
4. Open your browser and navigate to `http://localhost:7071/api/docs`
5. Use the Swagger UI to test the endpoints

Notes:

- The project targets **.NET 10** and uses the **isolated worker model** (`dotnet-isolated`).
- The in-memory `AssetStore` is registered as a singleton, so data persists for the lifetime of the host process.
