FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /app

# Copy application projects
COPY *.sln .
COPY src/NHSD.BuyingCatalogue.Documents.API/*.csproj ./src/NHSD.BuyingCatalogue.Documents.API/

# Copy full solution over
COPY . .

RUN dotnet build

# Publish the API
FROM build AS publish
WORKDIR /app/src/NHSD.BuyingCatalogue.Documents.API
RUN dotnet publish -c Release -o out

# Run the API
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS runtime
WORKDIR /app
COPY --from=publish /app/src/NHSD.BuyingCatalogue.Documents.API/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "NHSD.BuyingCatalogue.Documents.API.dll"]
