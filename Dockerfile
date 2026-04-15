FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Gestao-FDC.csproj ./
COPY Tests/Gestao_FDC.Tests.csproj Tests/
RUN dotnet restore Gestao-FDC.csproj

COPY . .
RUN dotnet publish Gestao-FDC.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Gestao-FDC.dll"]
