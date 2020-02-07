# BuyingCatalogueDocumentService - Service architecture for the NHS Digital Buying Catalogue Document Service .Net Core application

## IMPORTANT NOTES!
**You can use either the latest version of Visual Studio or .NET CLI for Windows, Mac and Linux**.

### Architecture overview
This application uses **.NET core** to provide an API capable of running on Linux or Windows.
It interfaces with Azure Blob Storage, via the .Net Azure Storage SDK.

### Overview of the application code
This repo consists of one service using **.NET Core** and **Docker**.

It contains two main endpoints:

- api/v1/Solutions/{solutionId}/documents
  - Returns a collection of all documents for a given solution
- api/v1/Solutions/{solutionId}/documents/{filename}
  - Returns a stream containing the document for a given solution

Examples:

- api/v1/Solutions/100000-001/documents
  - Returns {'roadmap.pdf', 'integrations.pdf'}
- api/v1/Solutions/100000-001/documents/roadmap.pdf
  - Returns a file stream containing roadmap.pdf

The application is broken down into the following project libraries:

- API
  - Defines and exposes the available Buying Catalogue Document Service endpoints
- API.UnitTests
  - Contains all unit tests for the API project
- API.IntegrationTests
  - Contains all integration tests for the API project

## Setting up your development environment for the Buying Catalogue Document Service

### Requirements

- .NET Core Version 3.1
- Docker

> Before you begin please install **.NET Core 3.1** & **Docker** on your machine.

## Running the API

### Running
To start up the API, run the following command from the root directory of the repository.

```bash
docker-compose up --build -d
```

This will start the API in a docker container, as well as Azure Storage, emulated within
another container. It will also populate the emulated Azure Storage with the following
files:

- 100000-001
  - integrations.pdf
  - roadmap.pdf
- 100000-002
  - integrations.pdf
  - roadmap.pdf
- 100000-003
  - integrations.pdf
  - roadmap.pdf

You can verify that the API has launched correctly by navigating to the following urls via any web browser.

- <http://localhost:8090/api/v1/Solutions/health/live>
- <http://localhost:8090/api/v1/Solutions/health/ready>

If both URLs return 'Healthy', the environment is configured correctly, and can be accessed via the public endpoints.

If the ready URL returns 'Unhealthy', the Azure Storage container has failed to launch, or cannot be accessed.

### Stopping
To stop the API, run the following command from the root directory of the repository.

```bash
docker-compose down -v
```

This will stop both the API docker container and the Azure Storage docker container.

## Running the Integration Tests

Before running the integration tests for the solution, the API must be started using the command given above.

These tests are written using SpecFlow, and can be run using any Unit Test runner capable of running NUnit tests.
