# Use .NET 7 SDK for building the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy solution and projects files first to leverage caching
COPY ["BackEnd.sln", "./"]
COPY ["URLShortener/*.csproj", "URLShortener/"]

# Restore NuGet packages
RUN dotnet restore "URLShortener/URLShortener.csproj"

# Copy the remaining source code
COPY . .

# Build and publish the application
RUN dotnet publish "URLShortener/URLShortener.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use a lightweight .NET runtime image for deployment
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port (change if needed)
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "URLShortener.dll"]
