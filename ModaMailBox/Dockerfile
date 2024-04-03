FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source
COPY . .
RUN dotnet restore --use-current-runtime ModaMailBox/ModaMailBox.csproj
RUN dotnet publish ModaMailBox/ModaMailBox.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app .
COPY ModaMailBox/docker-entrypoint.sh /docker-entrypoint.sh
COPY ModaMailBox/clamdtest.sh /app/clamdtest.sh
COPY ModaMailBox/clamd.remote.conf /app/clamd.remote.conf
RUN apt-get update && apt-get install -y gettext-base clamav-daemon && apt-get clean && chmod +x /docker-entrypoint.sh && chmod +x /app/clamdtest.sh
ENTRYPOINT ["/docker-entrypoint.sh"]