#!/bin/sh
envsubst < /app/clamd.remote.conf > /app/clamd.remote.conf2
rm /app/clamd.remote.conf
mv /app/clamd.remote.conf2 /app/clamd.remote.conf
cat /app/clamd.remote.conf >> /etc/clamav/clamd.conf
sed -i 's/\r$//' /app/clamdtest.sh
sed -i 's/\(^LocalSocket .*$\)/#\1/' /etc/clamav/clamd.conf
sed -i 's/\r$//' /etc/clamav/clamd.conf
envsubst < /app/appsettings.json > /app/appsettings2.json
rm /app/appsettings.json
mv /app/appsettings2.json /app/appsettings.json
dotnet ModaMailBox.dll
