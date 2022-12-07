FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder

COPY . .
WORKDIR StoreService/StoreHangfireService
RUN dotnet publish "StoreHangfireService.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=builder /app .
EXPOSE 51002

ENTRYPOINT ["dotnet", "StoreHangfireService.dll"]