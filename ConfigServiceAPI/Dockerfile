# Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore

# Publish the app. Make sure the published files are owned by root and
# are readable/executable by any UID. Create a writable logs folder.
RUN dotnet publish -c Release -o /app/publish \
    && chown -R root:root /app/publish \
    && chmod -R a+rX /app/publish \
    && find /app/publish -type d -exec chmod 0755 {} \; \
    && find /app/publish -type f -exec chmod 0644 {} \; \
    && mkdir -p /app/publish/logs \
    && chmod -R a+rwX /app/publish/logs

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 80

# Copy published app from build stage
COPY --from=build /app/publish .

# Ensure runtime directories are accessible/writable by an arbitrary UID
# (OpenShift/OKD runs containers with a random high UID). Do NOT set USER.
RUN mkdir -p /app/logs \
    && chmod -R a+rwX /app/logs \
    && chmod -R a+rX /app \
    && chmod a+rw /tmp || true

ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "ConfigServiceAPI.dll"]
