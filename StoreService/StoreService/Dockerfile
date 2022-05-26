FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as builder

COPY . .
WORKDIR StoreService/StoreService
RUN dotnet publish "StoreService.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=builder /app .
EXPOSE 80

ENTRYPOINT ["dotnet", "StoreService.dll"]