# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["CadCliX/CadCliX.csproj", "CadCliX/"]
RUN dotnet restore "CadCliX/CadCliX.csproj"

# Copy the rest of the code and build
COPY . .
WORKDIR "/src/CadCliX"
RUN dotnet build "CadCliX.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "CadCliX.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Set environment to Development to enable Swagger
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_HTTP_PORTS=8080

# Copy published app
COPY --from=publish /app/publish .

# Set entry point
ENTRYPOINT ["dotnet", "CadCliX.dll"]
