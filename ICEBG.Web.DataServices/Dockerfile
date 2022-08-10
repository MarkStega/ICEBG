#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG version=2022-01-01--13-00-00--WIP
WORKDIR /src
COPY ["NuGet.Config", "."]
COPY ["ICEBG.Web.DataServices/ICEBG.Web.DataServices.csproj", "ICEBG.Web.DataServices/"]
COPY ["ICEBG.Client/ICEBG.Client.csproj", "ICEBG.Client/"]
COPY ["DataTier/DataTier.csproj", "DataTier/"]
COPY ["SystemFramework/SystemFramework.csproj", "SystemFramework/"]
COPY ["SharedUtilities/SharedUtilities.csproj", "SharedUtilities/"]
RUN dotnet restore --disable-parallel "ICEBG.Web.DataServices/ICEBG.Web.DataServices.csproj"
COPY . .
WORKDIR "/src/ICEBG.Web.DataServices"
RUN dotnet publish "ICEBG.Web.DataServices.csproj" -c Azure -o /app/publish -p:Version=$version

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ICEBG.Web.DataServices.dll"]