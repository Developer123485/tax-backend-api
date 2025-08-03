# Specify the base image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

# Set the working directory
WORKDIR /app

# Copy csproj and restore dependencies
COPY TaxAPI/*.csproj ./
#RUN dotnet restore

# Copy the entire project and build the application
#COPY TaxApp.BAL/ /app/TaxApp.BAL/
COPY . ./
RUN dotnet publish TaxAPI/TaxAPI.csproj -c Release -o out

# Specify the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0

# Set the working directory
WORKDIR /app

# Copy the built application from the build environment
COPY --from=build-env /app/out .
COPY TaxAPI/EmailTemplates ./EmailTemplates/
#RUN apt-get update
#RUN apt-get install chromium -y
ENTRYPOINT ["dotnet", "/app/TaxAPI.dll"]
