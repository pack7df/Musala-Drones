#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Musala.Drones.ApiHost/Musala.Drones.ApiHost.csproj", "Musala.Drones.ApiHost/"]
COPY ["Musala.Drones.MongoInfrastructure/Musala.Drones.MongoInfrastructure.csproj", "Musala.Drones.MongoInfrastructure/"]
COPY ["Musala.Drones.Domain/Musala.Drones.Domain.csproj", "Musala.Drones.Domain/"]
RUN dotnet restore "Musala.Drones.ApiHost/Musala.Drones.ApiHost.csproj" --disable-parallel
COPY . .
WORKDIR "/src/Musala.Drones.ApiHost"
RUN dotnet build "Musala.Drones.ApiHost.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Musala.Drones.ApiHost.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Musala.Drones.ApiHost.dll"]