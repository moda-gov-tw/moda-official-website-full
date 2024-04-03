FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source
COPY . .
RUN dotnet restore --use-current-runtime ConsoleApp/ConsoleApp.csproj
RUN dotnet publish ConsoleApp/ConsoleApp.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app .
COPY ConsoleApp/docker-entrypoint.sh /docker-entrypoint.sh
COPY ConsoleApp/push.sh /app/push.sh 
RUN apt-get update && apt-get upgrade -y && apt-get install -y gettext-base && apt-get clean
ENTRYPOINT ["/docker-entrypoint.sh"]