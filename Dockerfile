FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /src
COPY . .
WORKDIR /src/BookRec.api
RUN dotnet publish WebApplication1.csproj -c Release -o /app/out
WORKDIR /app
EXPOSE 8080
ENTRYPOINT ["dotnet", "out/WebApplication1.dll"]