#!/bin/bash
sed -i 's/\r$//' /app/push.sh
envsubst < /app/appsettings.json > /app/appsettings2.json
rm /app/appsettings.json
mv /app/appsettings2.json /app/appsettings.json
dotnet ConsoleApp.dll
/app/push.sh