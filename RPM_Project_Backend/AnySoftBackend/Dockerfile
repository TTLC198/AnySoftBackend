FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RPM_Project_Backend/RPM_Project_Backend.csproj", "RPM_Project_Backend/"]
RUN dotnet restore "RPM_Project_Backend/RPM_Project_Backend.csproj"
COPY . .
WORKDIR "/src/RPM_Project_Backend"
RUN dotnet build "RPM_Project_Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RPM_Project_Backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RPM_Project_Backend.dll"]
