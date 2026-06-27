#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["HugeMazes.ConsoleApp/HugeMazes.ConsoleApp.csproj", "HugeMazes.ConsoleApp/"]
COPY ["HugeMazes/HugeMazes.csproj", "HugeMazes/"]
RUN dotnet restore "HugeMazes.ConsoleApp/HugeMazes.ConsoleApp.csproj"
COPY . .
WORKDIR "/src/HugeMazes.ConsoleApp"
RUN dotnet build "HugeMazes.ConsoleApp.csproj" -c Release -o /app/build

FROM build AS publish
ARG BUILD_VERSION
ARG VER=${BUILD_VERSION:-1.0.0}
RUN dotnet publish "HugeMazes.ConsoleApp.csproj" -o /app/publish /p:Version=$VER

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HugeMazes.ConsoleApp.dll"]
