FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
RUN ls -la 
RUN ls -la src
#COPY src/MarginTrading.AssetService/entrypoint.sh app/entrypoint.sh
WORKDIR /app
COPY . .
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
#ENTRYPOINT ["dotnet", "MarginTrading.AssetService.dll"]
#RUN ls -la src
RUN ls -la 
ENTRYPOINT ["entrypoint.sh"]