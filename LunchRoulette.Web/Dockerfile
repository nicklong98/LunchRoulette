FROM nicklong98/dotnet-build-env:2.1 as build-env

COPY . /app
WORKDIR /app/LunchRoulette.Web

RUN dotnet publish -c Release -o ../out


FROM microsoft/dotnet:2.1-runtime
COPY --from=build-env /app/out /app

WORKDIR /app
ENTRYPOINT [ "dotnet", "LunchRoulette.Web.dll"]