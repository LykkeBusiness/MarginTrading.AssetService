FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY . .
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
#ENTRYPOINT ["dotnet", "MarginTrading.AssetService.dll"]
RUN ls -la /src
RUN ls -la 
COPY src/entrypoint.sh entrypoint.sh
ENTRYPOINT ["entrypoint.sh"]