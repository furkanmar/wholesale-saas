FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Önce sadece csproj'ları kopyala — restore cache'i için
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["src/Domain/Domain.csproj",                     "src/Domain/"]
COPY ["src/Application/Application.csproj",           "src/Application/"]
COPY ["src/Infrastructure/Infrastructure.csproj",     "src/Infrastructure/"]
COPY ["src/Shared/Shared.csproj",                     "src/Shared/"]
COPY ["src/Web/Web.csproj",                           "src/Web/"]

RUN dotnet restore "src/Web/Web.csproj"

# Kaynak kodu kopyala ve publish et
COPY . .
RUN dotnet publish "src/Web/Web.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Wholesale.Web.dll"]
