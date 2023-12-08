FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
#COPY src/MarginTrading.AssetService/entrypoint.sh app/entrypoint.sh
WORKDIR /src
RUN ls -la 
WORKDIR /app
COPY . .
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
#ENTRYPOINT ["dotnet", "MarginTrading.AssetService.dll"]
#RUN ls -la src
RUN ls -la ../
ENTRYPOINT ["entrypoint.sh"]