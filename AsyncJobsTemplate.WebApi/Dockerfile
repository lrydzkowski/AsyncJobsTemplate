FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AsyncJobsTemplate.WebApi/AsyncJobsTemplate.WebApi.csproj", "AsyncJobsTemplate.WebApi/"]
COPY ["AsyncJobsTemplate.Core/AsyncJobsTemplate.Core.csproj", "AsyncJobsTemplate.Core/"]
COPY ["AsyncJobsTemplate.Shared/AsyncJobsTemplate.Shared.csproj", "AsyncJobsTemplate.Shared/"]
COPY ["AsyncJobsTemplate.Infrastructure/AsyncJobsTemplate.Infrastructure.csproj", "AsyncJobsTemplate.Infrastructure/"]
RUN dotnet restore "AsyncJobsTemplate.WebApi/AsyncJobsTemplate.WebApi.csproj"
COPY . .
WORKDIR "/src/AsyncJobsTemplate.WebApi"
RUN dotnet build "AsyncJobsTemplate.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AsyncJobsTemplate.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AsyncJobsTemplate.WebApi.dll"]
