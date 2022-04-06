FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Maets/Maets.csproj", "Maets/"]
RUN dotnet restore "Maets/Maets.csproj"
COPY . .
WORKDIR "/src/Maets"
RUN dotnet build "Maets.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Maets.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Maets.dll"]
