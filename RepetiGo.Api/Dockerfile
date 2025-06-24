# Pull .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /App

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /App
ENV ASPNETCORE_ENVIRONMENT=Development
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "RepetiGo.Api.dll"]