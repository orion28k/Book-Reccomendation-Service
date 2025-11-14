# Start with .NET 9.0 to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /app

# Copy everything from the project
COPY . .

# Go to the API folder and build the app
WORKDIR /app/src/BookRec.Api
RUN dotnet publish BookRec.Api.csproj -c Release -o /app/out

# Go back to the app folder
WORKDIR /app

# Tell Docker the app uses port 8080
EXPOSE 8080

# Tell ASP.NET to actually listen on 8080
ENV ASPNETCORE_URLS=http://+:8080

# Start the app
ENTRYPOINT ["dotnet", "out/BookRec.Api.dll"]
