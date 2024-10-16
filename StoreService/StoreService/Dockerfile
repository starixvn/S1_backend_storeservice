FROM hub.lumigame.com/base/dotnet/core/sdk:6.0 AS builder

COPY . .

WORKDIR StoreService/StoreService

RUN dotnet publish "StoreService.csproj" -c Release -o /app

FROM hub.lumigame.com/base/dotnet/core/sdk:6.0

WORKDIR /app

COPY --from=builder /app .

EXPOSE 5000

ENTRYPOINT ["dotnet", "StoreService.dll"]