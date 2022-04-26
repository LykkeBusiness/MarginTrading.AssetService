FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY . .
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
#ENTRYPOINT ["dotnet", "MarginTrading.AssetService.dll"]
ENTRYPOINT ["./entrypoint.sh"]