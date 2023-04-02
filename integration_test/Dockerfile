#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
#WORKDIR /app
#
#FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
#WORKDIR /src
#COPY ["IntegrationTest/IntegrationTest.csproj", "IntegrationTest/"]
#RUN dotnet restore "IntegrationTest/IntegrationTest.csproj"
#COPY . .
#WORKDIR "/src/IntegrationTest"
#RUN dotnet build "IntegrationTest.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "IntegrationTest.csproj" -c Release -o /app/publish /p:UseAppHost=false
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "IntegrationTest.dll"]

#FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
#WORKDIR /app
#
#FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
#WORKDIR /src
#COPY ["IntegrationTest/IntegrationTest.csproj", "IntegrationTest/"]
#RUN dotnet restore "IntegrationTest/IntegrationTest.csproj"
#COPY . .
#WORKDIR "/src/IntegrationTest"
#RUN dotnet build "IntegrationTest.csproj" -c Release -o /app/build
#
## Run the tests in the container
#RUN dotnet test "IntegrationTest.csproj" -c Release
#
#FROM base AS final
#WORKDIR /app
#COPY --from=build /app/build .
#ENTRYPOINT ["dotnet", "IntegrationTest.dll"]

#
#FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
#WORKDIR /src
#COPY ["IntegrationTest/IntegrationTest.csproj", "IntegrationTest/"]
#RUN dotnet restore "IntegrationTest/IntegrationTest.csproj"
#COPY . .
#WORKDIR "/src/IntegrationTest"
#RUN dotnet build "IntegrationTest.csproj" -c Release -o /app/build
#
#FROM build AS testrunner
#WORKDIR "/src/IntegrationTest"
#ENTRYPOINT ["dotnet", "test", "--no-restore", "--logger", "trx"]
#
#FROM mcr.microsoft.com/dotnet/runtime:6.0 AS final
#WORKDIR /app
#COPY --from=build /app/build .
#COPY --from=testrunner /src/IntegrationTest/TestResults /TestResults
#ENTRYPOINT ["dotnet", "IntegrationTest.dll"]
#


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["IntegrationTest/IntegrationTest.csproj", "IntegrationTest/"]
RUN dotnet restore "IntegrationTest/IntegrationTest.csproj"
COPY . .
WORKDIR "/src/IntegrationTest"
RUN dotnet build "IntegrationTest.csproj" -c Release -o /app/build
RUN dotnet test "IntegrationTest.csproj" --logger:trx;LogFileName=test_results.trx

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS final
WORKDIR /app
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "IntegrationTest.dll"]
