FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source
COPY . .
RUN dotnet restore --use-current-runtime ConsoleApp-Create/ConsoleApp.csproj
RUN dotnet publish ConsoleApp-Create/ConsoleApp.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app .
COPY ConsoleApp-Create/docker-entrypoint.sh /docker-entrypoint.sh
RUN apt-get update && apt-get upgrade -y && apt-get install -y gettext-base && apt-get clean && chmod +x /docker-entrypoint.sh
ENTRYPOINT ["/docker-entrypoint.sh"]
