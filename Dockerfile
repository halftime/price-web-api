


# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["price-web-api.csproj", "./"]
RUN dotnet restore "price-web-api.csproj"
COPY . .
RUN dotnet publish "price-web-api.csproj" -c Release -o /app/publish

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "price-web-api.dll"]

EXPOSE 5080
EXPOSE 5081