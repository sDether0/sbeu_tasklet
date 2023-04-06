#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["SBEU.Tasklet/SBEU.Tasklet.Api/SBEU.Tasklet.Api.csproj", "SBEU.Tasklet/SBEU.Tasklet.Api/"]
COPY ["SBEU.Exception/SBEU.Exceptions.csproj", "SBEU.Exception/"]
COPY ["SBEU.Tasklet/SBEU.Tasklet.DataLayer/SBEU.Tasklet.DataLayer.csproj", "SBEU.Tasklet/SBEU.Tasklet.DataLayer/"]
COPY ["SBEU.Tasklet/SBEU.Tasklet.Models/SBEU.Tasklet.Models.csproj", "SBEU.Tasklet/SBEU.Tasklet.Models/"]
RUN dotnet restore "SBEU.Tasklet/SBEU.Tasklet.Api/SBEU.Tasklet.Api.csproj"
COPY . .
WORKDIR "/src/SBEU.Tasklet/SBEU.Tasklet.Api"
RUN dotnet build "SBEU.Tasklet.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SBEU.Tasklet.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SBEU.Tasklet.Api.dll"]