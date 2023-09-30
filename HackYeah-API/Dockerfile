FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["HackYeah-API/HackYeah-API.csproj", "HackYeah-API/"]
RUN dotnet restore "HackYeah-API/HackYeah-API.csproj"
COPY . .
WORKDIR "/src/HackYeah-API"
RUN dotnet build "HackYeah-API.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "HackYeah-API.csproj" -c Release -o /app/publish
COPY ./hackathon.db /app/publish/hackathon.db
COPY ./Databases/Queries /app/Databases/Queries

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HackYeah-API.dll"]
