# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["UserServiceAPI/UserServiceAPI.csproj", "UserServiceAPI/"]
COPY ["UserService.Application/UserService.Application.csproj", "UserService.Application/"]
COPY ["UserService.Domain/UserService.Domain.csproj", "UserService.Domain/"]
COPY ["UserService.Infrastructure/UserService.Infrastructure.csproj", "UserService.Infrastructure/"]

RUN dotnet restore "UserServiceAPI/UserServiceAPI.csproj"

COPY . .

WORKDIR "/src/UserServiceAPI"

RUN dotnet publish "UserServiceAPI.csproj" \
    -c Release \
    -o /app/publish

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "UserServiceAPI.dll"]