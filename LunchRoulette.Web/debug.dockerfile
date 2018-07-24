FROM nicklong98/dotnet-build-env:2.1 AS build-env

COPY . /app
WORKDIR /app/LunchRoulette.Web
RUN dotnet publish -c Debug -o ../out

FROM nicklong98/dotnet-build-env:2.1
COPY --from=build-env /app/out /app
COPY --from=build-env /app/LunchRoulette.Web/ClientApp /app/ClientApp
WORKDIR /app
ENTRYPOINT ["dotnet", "LunchRoulette.Web.dll"]