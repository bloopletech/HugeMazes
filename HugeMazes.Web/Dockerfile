#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["HugeMazes.Web/HugeMazes.Web.csproj", "HugeMazes.Web/"]
COPY ["HugeMazes/HugeMazes.csproj", "HugeMazes/"]
RUN dotnet restore "HugeMazes.Web/HugeMazes.Web.csproj"
COPY . .
WORKDIR "/src/HugeMazes.Web"
RUN dotnet build "HugeMazes.Web.csproj" -c Release -o /app/build

FROM build AS publish
ARG BUILD_VERSION
ARG VER=${BUILD_VERSION:-1.0.0}
RUN dotnet publish "HugeMazes.Web.csproj" -c Release -o /app/publish /p:Version=$VER /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HugeMazes.Web.dll"]
