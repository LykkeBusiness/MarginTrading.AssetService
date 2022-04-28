FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
#COPY src/MarginTrading.AssetService/entrypoint.sh app/entrypoint.sh
WORKDIR /app
COPY . .
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
RUN apt-get update
RUN apt-get install wget -y
RUN wget https://download.jetbrains.com/resharper/dotUltimate.2022.1/JetBrains.dotCover.CommandLineTools.linux-x64.2022.1.tar.gz && \
    tar xvf JetBrains.dotCover.CommandLineTools.linux-x64.2022.1.tar.gz 
#ENTRYPOINT ["dotnet", "MarginTrading.AssetService.dll"]
#RUN ls -la src
RUN ls -la 
ENTRYPOINT ["entrypoint.sh"]