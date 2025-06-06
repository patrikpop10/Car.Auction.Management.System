# Stage 1: Build and Publish
FROM mcr.microsoft.com/dotnet/nightly/sdk:10.0-preview AS build
WORKDIR /app

# Copy the main project and all its dependencies
COPY src/ ./src/

# Restore dependencies for the main project only
RUN dotnet restore src/Api/Api.csproj

# Build and publish the main project
RUN dotnet publish src/Api/Api.csproj -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/nightly/aspnet:10.0-preview AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose port 80 (default for ASP.NET)
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# Run the web app
ENTRYPOINT ["dotnet", "Api.dll"]