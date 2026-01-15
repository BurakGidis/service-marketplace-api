# --- BUILD STAGE ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["ServiceMarketAPI/ServiceMarketAPI.csproj", "ServiceMarketAPI/"]
RUN dotnet restore "ServiceMarketAPI/ServiceMarketAPI.csproj"

COPY . .
WORKDIR "/src/ServiceMarketAPI"
RUN dotnet publish "ServiceMarketAPI.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

# --- RUNTIME STAGE ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ServiceMarketAPI.dll"]
