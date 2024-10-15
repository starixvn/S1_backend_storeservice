ARG CI_REGISTRY_IMAGE

FROM $CI_REGISTRY_IMAGE:builder as deps

RUN mkdir -p /src/packages

FROM hub.lumigame.com/base/dotnet/core/sdk:6.0 AS builder

ENV NUGET_PACKAGES=/src/packages

COPY --from=deps $NUGET_PACKAGES/. $NUGET_PACKAGES/

COPY . .
WORKDIR StoreService/StoreService
RUN dotnet publish "StoreService.csproj" -c Release -o /app

RUN dotnet restore -v n -r linux-x64 ./StoreService/StoreService.csproj

FROM hub.lumigame.com/base/dotnet/core/sdk:6.0
WORKDIR /app
COPY --from=builder /app .
RUN dotnet publish -r linux-x64 --self-contained false -o /app
EXPOSE 5000

ENTRYPOINT ["dotnet", "StoreService.dll"]