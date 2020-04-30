# BuyingCatalogueDocumentService - Service architecture for the NHS Digital Buying Catalogue Document Service .Net Core application

## IMPORTANT NOTES!
**You can use either the latest version of Visual Studio or .NET CLI for Windows, Mac and Linux**.

### Architecture overview
This application uses **.NET core** to provide an API capable of running on Linux or Windows.
It interfaces with Azure Blob Storage, via the .Net Azure Storage SDK.

### Overview of the application code
This repo consists of one service using **.NET Core** and **Docker**.

List of all endpoints the application exposes can be found [here](http://localhost:5201/swagger/index.html)

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
docker-compose -f "docker-compose.integration.yml" up -d
```

This will start the API in a docker container, as well as Azure Storage, emulated within
another container.

You can verify that the API has launched correctly by navigating to the following urls via any web browser.

- <http://localhost:5201/health/live>
- <http://localhost:5201/health/ready>

If both URLs return 'Healthy', the environment is configured correctly, and can be accessed via the public endpoints.

If the ready URL returns 'Degraded', the Azure Storage container has failed to launch, or cannot be accessed.

### Stopping
To stop the API, run the following command from the root directory of the repository.

```bash
docker-compose -f "docker-compose.integration.yml" down -v
```

This will stop both the API docker container and the Azure Storage docker container.

## Running the Integration Tests

Before running the integration tests for the solution, the API must be started using the command given above.

These tests are written using SpecFlow, and can be run using any Unit Test runner capable of running NUnit tests.
