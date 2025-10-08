# -------- build stage --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY TaskManagement.sln ./
COPY TaskManagement/TaskManagement.Web.csproj TaskManagement/
COPY TaskManagement.Application/TaskManagement.Application.csproj TaskManagement.Application/
COPY TaskManagement.Infrastructure/TaskManagement.Infrastructure.csproj TaskManagement.Infrastructure/
COPY TakManagement.Domain/TaskManagement.Domain.csproj TakManagement.Domain/
COPY TaskManagement.Tests/TaskManagement.Tests.csproj TaskManagement.Tests/

RUN dotnet restore TaskManagement.sln

COPY . .
RUN dotnet publish TaskManagement/TaskManagement.Web.csproj -c Release -o /app/out

# -------- runtime stage --------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/out .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "TaskManagement.Web.dll"]
