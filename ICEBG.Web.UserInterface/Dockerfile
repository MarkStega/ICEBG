#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

ARG version
ENV VERSION $version

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NuGet.Config", "."]
COPY ["ICEBG.Web.UserInterface/ICEBG.Web.UserInterface.csproj", "ICEBG.Web.UserInterface/"]
COPY ["ICEBG.Client/ICEBG.Client.csproj", "ICEBG.Client/"]
COPY ["DataTier/DataTier.csproj", "DataTier/"]
COPY ["SystemFramework/SystemFramework.csproj", "SystemFramework/"]
COPY ["SharedUtilities/SharedUtilities.csproj", "SharedUtilities/"]
RUN dotnet restore --disable-parallel "ICEBG.Web.UserInterface/ICEBG.Web.UserInterface.csproj"
COPY . .
WORKDIR "/src/ICEBG.Web.UserInterface"
RUN dotnet publish "ICEBG.Web.UserInterface.csproj" -c Azure -o /app/publish -p:Version=$VERSION

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ICEBG.Web.UserInterface.dll"]