#!/bin/bash
envsubst < /app/appsettings.json > /app/appsettings2.json
rm /app/appsettings.json
mv /app/appsettings2.json /app/appsettings.json
envsubst < /app/wwwroot/Oauth/brave-attic.json > /app/wwwroot/Oauth/brave-attic2.json
rm /app/wwwroot/Oauth/brave-attic.json
mv /app/wwwroot/Oauth/brave-attic2.json /app/wwwroot/Oauth/brave-attic.json
envsubst < /app/wwwroot/Oauth/GA4.json > /app/wwwroot/Oauth/GA42.json
rm /app/wwwroot/Oauth/GA4.json
mv /app/wwwroot/Oauth/GA42.json /app/wwwroot/Oauth/GA4.json
dotnet Management.dll
