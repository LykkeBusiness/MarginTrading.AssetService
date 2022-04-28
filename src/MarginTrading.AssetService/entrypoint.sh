#!/bin/bash -l
set -eu

./dotCover.sh cover --output="/app/logs/assetservice/test.html" --reportType="HTML" --targetExecutable="/usr/share/dotnet/dotnet" --targetArguments="./MarginTrading.AssetService.dll" --startInstance="1"
./dotCover.sh send --Instance="1" --Command="Cover"