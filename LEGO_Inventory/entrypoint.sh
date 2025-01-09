#!/bin/bash

set -e
#run_cmd="dotnet run --server.urls http://*:8080"

until dotnet ef database update; do
>&2 echo "SQL Server is starting up"
sleep 5
done

>&2 echo "SQL Server is up - executing command"
exec $run_cmd
sleep 10

exec "$@