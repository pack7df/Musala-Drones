docker stop host
docker stop db
docker image rm musala_host --force
docker-compose up -d
docker stop host
dotnet test --blame