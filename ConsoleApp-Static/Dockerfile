FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source
COPY . .
RUN dotnet restore --use-current-runtime ConsoleApp-Static/ConsoleApp.csproj
RUN dotnet publish ConsoleApp-Static/ConsoleApp.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app .
COPY ConsoleApp-Static/docker-entrypoint.sh /docker-entrypoint.sh
COPY ConsoleApp-Static/push.sh /app
RUN apt-get update && apt-get upgrade -y && apt-get install -y gettext-base ssh git rsync && apt-get clean && chmod +x /docker-entrypoint.sh && chmod +x /app/push.sh
ENTRYPOINT ["/docker-entrypoint.sh"]