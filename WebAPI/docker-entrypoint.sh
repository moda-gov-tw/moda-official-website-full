#!/bin/bash
envsubst < /app/appsettings.json > /app/appsettings2.json
rm /app/appsettings.json
mv /app/appsettings2.json /app/appsettings.json
dotnet WebAPI.dll