FROM auriga-repo.nts.local:5001/dotnet/sdk:10.0 AS build-env

ENV NUGET_CERT_REVOCATION_MODE=offline

WORKDIR /app

ARG FETCH_CERTS=false
ARG FETCH_CERTS_FROM=auriga-repo.nts.local

RUN curl -k -s https://auriga-repo.nts.local/repository/raw-nts/certs/auriga-repo-root-ca.crt -o /etc/ssl/certs/auriga-ca.crt && update-ca-certificates && dotnet nuget list source

COPY . ./

RUN dotnet publish CumList.DataService -a x64 --sc false -o /publish -c Release

FROM auriga-repo.nts.local:5001/dotnet/aspnet:10.0
WORKDIR /app

ENV DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false

COPY --from=build-env /publish .

ENTRYPOINT ["dotnet", "CumList.DataService.dll"]