FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FileServices/FileServices.csproj", "FileServices/"]
COPY ["Services/Services.csproj", "Services/"]
RUN dotnet restore "FileServices/FileServices.csproj"
COPY . .
WORKDIR "/src/FileServices"
RUN dotnet build "FileServices.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FileServices.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY FileServices/docker-entrypoint.sh /docker-entrypoint.sh
RUN apt-get update && apt-get upgrade -y && apt-get install -y gettext-base && apt-get clean
ENTRYPOINT ["/docker-entrypoint.sh"]