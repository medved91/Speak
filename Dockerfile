FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Install Node.js
RUN curl -fsSL https://deb.nodesource.com/setup_14.x | bash - \
    && apt-get install -y \
        nodejs \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /src
COPY ["Speak.Web/Speak.Web.csproj", "Speak.Web/"]
RUN dotnet restore "Speak.Web/Speak.Web.csproj"
COPY . .
WORKDIR "/src/Speak.Web"
RUN dotnet build "Speak.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Speak.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

COPY .cert/cert.pem /usr/local/share/ca-certificates/cert.pem
RUN update-ca-certificates

ENV ASPNETCORE_URLS=https://+:7135

EXPOSE 7135

ENTRYPOINT ["dotnet", "Speak.Web.dll"]

