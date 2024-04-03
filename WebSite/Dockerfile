FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source
COPY . .
RUN dotnet restore --use-current-runtime WebSite/WebSite.csproj
RUN dotnet publish WebSite/WebSite.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app .
COPY WebSite/docker-entrypoint.sh /docker-entrypoint.sh
RUN apt-get update && apt-get upgrade -y && apt-get install -y gettext-base ssh git && apt-get clean 
ENTRYPOINT ["/docker-entrypoint.sh"]