#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Helther/Server/Helther.Server.csproj", "Helther/Server/"]
COPY ["Helther/Client/Helther.Client.csproj", "Helther/Client/"]
COPY ["Helther/Shared/Helther.Shared.csproj", "Helther/Shared/"]
RUN dotnet restore "Helther/Server/Helther.Server.csproj"
COPY . .
WORKDIR "/src/Helther/Server"
RUN dotnet build "Helther.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Helther.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Helther.Server.dll"]