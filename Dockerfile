FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/IMS.Api/IMS.Api.csproj", "src/IMS.Api/"]
COPY ["src/IMS.Application/IMS.Application.csproj", "src/IMS.Application/"]
COPY ["src/IMS.Domain/IMS.Domain.csproj", "src/IMS.Domain/"]
COPY ["src/IMS.Infrastructure/IMS.Infrastructure.csproj", "src/IMS.Infrastructure/"]

RUN dotnet restore "src/IMS.Api/IMS.Api.csproj"

COPY . .
WORKDIR "/src/src/IMS.Api"
RUN dotnet publish "IMS.Api.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "IMS.Api.dll"]
