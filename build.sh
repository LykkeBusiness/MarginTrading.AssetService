#!/bin/bash

# Define NuGet feeds
PRIVATE_FEED="https://nuget-lykkecloud.azurewebsites.net/nuget"
PUBLIC_FEED="https://api.nuget.org/v3/index.json"

# Define your solution path
SOLUTION_PATH="./MarginTrading.AssetService.sln"

# Add NuGet source for the private feed
added_lykke_source=false
if ! dotnet nuget list source | grep -q $PRIVATE_FEED; then
  dotnet nuget add source $PRIVATE_FEED --name $PRIVATE_FEED
  added_lykke_source=true
fi

# Restore NuGet packages and build the solution
dotnet restore $SOLUTION_PATH --source $PRIVATE_FEED --source $PUBLIC_FEED
dotnet build $SOLUTION_PATH -c Release

# Optionally remove the NuGet source after the build
if [ "$added_lykke_source" = true ]; then
  dotnet nuget remove source $PRIVATE_FEED
fi
