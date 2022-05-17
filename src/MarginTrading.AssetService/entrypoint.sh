#!/bin/bash

printenv

/app/dotCover.sh cover --output="/app/logs/assetservice/test.html" --reportType="HTML" --targetExecutable="/usr/share/dotnet/dotnet" --targetArguments="/app/MarginTrading.AssetService.dll" --startInstance="1"
/app/dotCover.sh send --Instance="1" --Command="Cover"
sleep infinity
