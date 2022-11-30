FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Play.Inventory.Services/Play.Inventory.Services.csproj", "Play.Inventory.Services/"]
RUN dotnet restore "Play.Inventory.Services/Play.Inventory.Services.csproj"
COPY . .
WORKDIR "/src/Play.Inventory.Services"
RUN dotnet build "Play.Inventory.Services.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Play.Inventory.Services.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Play.Inventory.Services.dll"]
