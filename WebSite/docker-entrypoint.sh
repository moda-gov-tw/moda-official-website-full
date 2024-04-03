#!/bin/bash
envsubst < /app/appsettings.json > /app/appsettings2.json
rm /app/appsettings.json
mv /app/appsettings2.json /app/appsettings.json

envsubst < /app/wwwroot/js/apiurl.js > /app/wwwroot/js/apiurl2.js
rm /app/wwwroot/js/apiurl.js
mv /app/wwwroot/js/apiurl2.js /app/wwwroot/js/apiurl.js

cp -rf /app/wwwroot/* /app/files/wwwroot
sync
rm -rf /app/wwwroot
ln -s /app/files/wwwroot /app
dotnet WebSite.dll