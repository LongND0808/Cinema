# Use the official .NET SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy the solution and project files
COPY Cinema.sln .
COPY Cinema.Common/Cinema.Common.csproj Cinema.Common/
COPY Cinema.Core/Cinema.Core.csproj Cinema.Core/
COPY Cinema.Domain/Cinema.Domain.csproj Cinema.Domain/
COPY Cinema.Infrastructure/Cinema.Infrastructure.csproj Cinema.Infrastructure/
COPY Cinema.Test/Cinema.Test.csproj Cinema.Test/
COPY Cinema.Web/Cinema.Web.csproj Cinema.Web/

# Restore dependencies
RUN dotnet restore

# Copy all the source code files
COPY . .

# Build the project
WORKDIR /app/Cinema.Web
RUN dotnet publish -c Release -o out

# Use the official runtime image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/Cinema.Web/out .

# Expose port 80 for the app
EXPOSE 80

# Define the entry point for the container
ENTRYPOINT ["dotnet", "Cinema.Web.dll"]
