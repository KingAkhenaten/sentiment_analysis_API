#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ClientGUI/ClientGUI.csproj", "ClientGUI/"]
COPY ["ClientGUIUnitTesting/ClientGUIUnitTesting.csproj", "ClientGUIUnitTesting/"]
COPY ["IntegrationTests/IntegrationTests.csproj", "IntegrationTests/"]
RUN dotnet restore "ClientGUI/ClientGUI.csproj"
RUN dotnet restore "ClientGUIUnitTesting/ClientGUIUnitTesting.csproj"
RUN dotnet restore "IntegrationTests/IntegrationTests.csproj"
COPY . .
WORKDIR "/src/ClientGUI"
RUN dotnet build "ClientGUI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ClientGUI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ClientGUI.dll"]

##FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
##WORKDIR /app
##COPY *.sln .
##COPY ["ClientGUIUnitTesting/ClientGUIUnitTesting.csproj", "ClientGUIUnitTesting/"]
##COPY ["IntegrationTests/IntegrationTests.csproj", "IntegrationTests/"]
##COPY ["ClientGUI/ClientGUI.csproj", "ClientGUI/"]
##RUN dotnet restore

##COPY . .
##RUN dotnet build

##run the unit tests, if they fail, the build will fail
##RUN dotnet test "ClientGUIUnitTesting/ClientGUIUnitTesting.csproj" --logger:trx

#run the unit tests, if they fail, the build will fail
# FROM build AS test
# WORKDIR /app/ClientGUIUnitTesting
# RUN dotnet test --logger:trx

# # run the integration tests, if they fail, the build will fail
# WORKDIR /app/IntegrationTests
# RUN dotnet test --logger:trx

## publish the app if the tests pass
##FROM build AS publish
##WORKDIR /app/ClientGUI
##RUN dotnet publish "ClientGUI.csproj" -c Release -o /app/publish

##FROM base AS final
##WORKDIR /app
##COPY --from=publish /app/publish .
##ENTRYPOINT ["dotnet", "ClientGUI.dll"]

#Assuming you are in the client-gui/ClientGUI folder
# docker build -f ./ClientGUI/Dockerfile . -t client-gui
# docker run -it --rm -p "32768:80" client-gui
