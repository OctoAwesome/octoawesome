#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["OctoAwesome.GameServer/OctoAwesome.GameServer.csproj", "OctoAwesome.GameServer/"]
COPY ["OctoAwesome.Network/OctoAwesome.Network.csproj", "OctoAwesome.Network/"]
COPY ["OctoAwesome.Runtime/OctoAwesome.Runtime.csproj", "OctoAwesome.Runtime/"]
COPY ["OctoAwesome/OctoAwesome.csproj", "OctoAwesome/"]
COPY ["OctoAwesome.Database/OctoAwesome.Database.csproj", "OctoAwesome.Database/"]
COPY ["OctoAwesome.Basics/OctoAwesome.Basics.csproj", "OctoAwesome.Basics/"]
RUN dotnet restore "OctoAwesome.GameServer/OctoAwesome.GameServer.csproj"
COPY . .
WORKDIR "/src/OctoAwesome.GameServer"
RUN dotnet build "OctoAwesome.GameServer.csproj" -c Release -o /app/build
RUN dotnet build "../OctoAwesome.Basics/OctoAwesome.Basics.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OctoAwesome.GameServer.csproj" -c Release -o /app/publish
RUN dotnet publish "../OctoAwesome.Basics/OctoAwesome.Basics.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OctoAwesome.GameServer.dll"]