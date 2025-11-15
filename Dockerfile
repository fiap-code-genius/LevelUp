# Estágio 1: Build (Compilação)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["LevelUp.sln", "."]
COPY ["LevelUp/LevelUp.csproj", "LevelUp/"]
COPY ["LevelUp.Application/LevelUp.Application.csproj", "LevelUp.Application/"]
COPY ["LevelUp.Domain/LevelUp.Domain.csproj", "LevelUp.Domain/"]
COPY ["LevelUp.Infra.Data/LevelUp.Infra.Data.csproj", "LevelUp.Infra.Data/"]
COPY ["LevelUp.Infra.IoC/LevelUp.Infra.IoC.csproj", "LevelUp.Infra.IoC/"]
COPY ["LevelUp.Api.Doc/LevelUp.Api.Doc.csproj", "LevelUp.Api.Doc/"]

RUN dotnet restore "LevelUp.sln"

COPY . .

WORKDIR "/src/LevelUp"
RUN dotnet publish "LevelUp.csproj" -c Release -o /app/publish --no-restore

# Estágio 2: Imagem Final (Execução)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

RUN useradd -m -s /bin/bash appuser
USER appuser

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "LevelUp.dll"]