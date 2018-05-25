FROM microsoft/dotnet:runtime AS base
WORKDIR /app
ENV ASPNETCORE_URLS http://*:80

FROM microsoft/dotnet:sdk AS build
WORKDIR /src
COPY . .
RUN dotnet restore
WORKDIR /src/src/SyboChallenge.Server.Gateway
RUN dotnet build --no-restore -c Release -o /app

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SyboChallenge.Server.Gateway.dll"]
