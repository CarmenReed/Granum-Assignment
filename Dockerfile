FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore src/Api/Api.csproj
RUN dotnet publish src/Api/Api.csproj -c Release -o /out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .
COPY interactions.db .
EXPOSE 8080
ENTRYPOINT ["/bin/sh", "-c", "dotnet Api.dll --urls http://0.0.0.0:${PORT:-8080}"]
