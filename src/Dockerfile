FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY *.sln .
COPY Glue.Delivery.Core/*.csproj Glue.Delivery.Core/
COPY Glue.Delivery.Core.UnitTests/*.csproj Glue.Delivery.Core.UnitTests/
COPY Glue.Delivery.WebApi/*.csproj Glue.Delivery.WebApi/
COPY Glue.Delivery.WebApi.IntegrationTests/*.csproj Glue.Delivery.WebApi.IntegrationTests/
RUN dotnet restore

COPY . .
WORKDIR /src
RUN dotnet build -c Release -o /app/build
RUN dotnet test -c Release 

FROM build AS publish
WORKDIR /src/Glue.Delivery.WebApi
RUN dotnet publish "Glue.Delivery.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Glue.Delivery.WebApi.dll"]
