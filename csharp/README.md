# serverless-csharp-labs

Hands-on labs for C# serverless applications on Azure

## Table of Contents

- [Overview](#overview)
- [Prerequisites](#prerequisites)
- [Lab Structure](#lab-structure)

## Overview

This repository contains a progressive learning series of hands-on labs for building serverless C# applications on Azure. Each lab builds upon the previous one, introducing new concepts and Azure services while maintaining best practices for production-ready applications.

**Technology Stack:** Azure Functions + ASP.NET Core + C# (.NET 10), designed for Azure Functions Consumption Plan following Azure Well-Architected Framework principles.

## Prerequisites

Before starting any lab, ensure you have:

- **Visual Studio Code** with the [Azure Functions extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions) and [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
- **Azure Functions Core Tools** (v4.x)
- **.NET 10 SDK** or later
- **Azure CLI** (`az`) – for Azure authentication (Labs 2+)
- **Azure Developer CLI** (`azd`) – for infrastructure (Labs 2+)
- **Azure subscription** with appropriate permissions

## Lab Structure

Each lab is independent with dedicated directories, separate ports (7071–7074), and isolated project files.

| Lab | Focus | Directory | Port | Description |
|-----|-------|-----------|------|-------------|
| **Lab 1** | Basic Functions | `solutions/lab1/` | 7071 | HTTP-triggered Azure Function with ASP.NET Core integration, data annotations validation, in-memory storage, and Swagger UI |

📖 **Each lab has its own detailed README with setup instructions and learning objectives.**

**Happy Learning!** 🚀

Start with Lab 1 and progress through each lab to build expertise in Azure serverless development with C#.
