# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file
COPY ["ScrumMovieTheater.csproj", "./"]

# Restore dependencies
RUN dotnet restore "ScrumMovieTheater.csproj"

# Copy the rest of the project
COPY . .

# Build and publish
RUN dotnet publish "ScrumMovieTheater.csproj" -c Release -o /app/publish /p:UseAppHost=false


# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# App Runner will provide the PORT environment variable
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ScrumMovieTheater.dll"]