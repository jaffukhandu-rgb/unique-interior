# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY . .

RUN dotnet restore Unique_1.csproj
RUN dotnet publish Unique_1.csproj -c Release -o /out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /out .

ENTRYPOINT ["dotnet", "Unique_1.dll"]