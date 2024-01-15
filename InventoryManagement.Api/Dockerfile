# Use the official .NET SDK image as the build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

# Set the working directory in the container
WORKDIR /app

# Copy the .csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the remaining source code
COPY . ./

# Build the application
RUN dotnet publish -c Release -o out

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Expose port 80 for the API to listen on
EXPOSE 80

# Define the command to run the application
ENTRYPOINT ["dotnet", "InventoryManagement.Api.dll"]
