FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /app
COPY . .
WORKDIR /app/WebApplication1
RUN dotnet publish WebApplication1.csproj -c Release -o /app/out
WORKDIR /app
EXPOSE 5000
ENTRYPOINT ["dotnet", "out/WebApplication1.dll"]