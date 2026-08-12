FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ContosoPizza/ContosoPizza.csproj ContosoPizza/
RUN dotnet restore ContosoPizza/ContosoPizza.csproj
COPY ContosoPizza/. ContosoPizza/
RUN dotnet publish ContosoPizza/ContosoPizza.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app ./
EXPOSE 8080
CMD ["sh", "-c", "ASPNETCORE_URLS=http://+:$PORT dotnet ContosoPizza.dll"]
