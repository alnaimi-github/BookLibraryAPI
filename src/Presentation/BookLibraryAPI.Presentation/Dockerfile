FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 9443


# Copy dev certificate
COPY ./src/Presentation/BookLibraryAPI.Presentation/aspnet-dev-cert.pfx /https/aspnet-dev-cert.pfx

RUN apt-get update && apt-get install -y postgresql-client redis-tools curl
# Copy wait-for-postgres script
COPY ../../wait-for-postgres.sh /app/wait-for-postgres.sh
RUN chmod +x /app/wait-for-postgres.sh

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Presentation/BookLibraryAPI.Presentation/BookLibraryAPI.Presentation.csproj", "src/Presentation/BookLibraryAPI.Presentation/"]
RUN dotnet restore "src/Presentation/BookLibraryAPI.Presentation/BookLibraryAPI.Presentation.csproj"
COPY . .
WORKDIR "/src/src/Presentation/BookLibraryAPI.Presentation"
RUN dotnet build "./BookLibraryAPI.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BookLibraryAPI.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["/bin/sh", "-c", "/app/wait-for-postgres.sh dotnet BookLibraryAPI.Presentation.dll"]
