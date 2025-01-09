FROM mcr.microsoft.com/dotnet/sdk:9.0 AS base
USER $APP_UID
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet tool install --global dotnet-ef --create-manifest-if-needed
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet tool install --global dotnet-ef --create-manifest-if-needed
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LEGO_Inventory.csproj", "./"]
RUN dotnet restore "LEGO_Inventory.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "LEGO_Inventory.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LEGO_Inventory.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
RUN dotnet ef database update

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY Migrations/ Migrations/.
COPY entrypoint.sh /usr/bin/

#ENTRYPOINT ["entrypoint.sh"]
CMD ["dotnet", "LEGO_Inventory.dll"]
