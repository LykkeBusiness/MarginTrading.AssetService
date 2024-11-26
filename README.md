
# MarginTrading.AssetService API #

API for settings management.

## How to use in prod env? ##

1. Pull "mt-settings-service" docker image with a corresponding tag.
2. Configure environment variables according to "Environment variables" section.
3. Put secrets.json with endpoint data including the certificate:
```json
"Kestrel": {
  "EndPoints": {
    "HttpsInlineCertFile": {
      "Url": "https://*:5110",
      "Certificate": {
        "Path": "<path to .pfx file>",
        "Password": "<certificate password>"
      }
    }
  }
}
```
4. Initialize all dependencies.
5. Run.

## How to run for debug? ##

1. Clone repo to some directory.
2. In MarginTrading.AssetService root create a appsettings.dev.json with settings.
3. Add environment variable "SettingsUrl": "appsettings.dev.json".
4. VPN to a corresponding env must be connected and all dependencies must be initialized.
5. Run.

### Dependencies ###

TBD

### Configuration ###

Kestrel configuration may be passed through appsettings.json, secrets or environment.
All variables and value constraints are default. For instance, to set host URL the following env variable may be set:
```json
{
    "Kestrel__EndPoints__Http__Url": "http://*:5010"
}
```

### Environment variables ###

* *RESTART_ATTEMPTS_NUMBER* - number of restart attempts. If not set int.MaxValue is used.
* *RESTART_ATTEMPTS_INTERVAL_MS* - interval between restarts in milliseconds. If not set 10000 is used.
* *SettingsUrl* - defines URL of remote settings or path for local settings.

### Settings ###

Settings schema is:
<!-- MARKDOWN-AUTO-DOCS:START (CODE:src=./template.json) -->
<!-- The below code snippet is automatically added from ./template.json -->
```json
{
  "APP_UID": "Integer",
  "ASPNETCORE_ENVIRONMENT": "String",
  "ASPNETCORE_ENVIRONMENT_TEST1": "String",
  "ENVIRONMENT": "String",
  "ENVIRONMENT_TEST1": "String",
  "Kestrel": {
    "EndPoints": {
      "Http": {
        "Url": "String"
      }
    }
  },
  "Logging": {
    "LogLevel": {
      "Microsoft": "String"
    }
  },
  "MarginTradingAssetService": {
    "AssetTypesWithZeroInterestRates": [
      "String"
    ],
    "BrokerId": "String",
    "BrokerSettingsChangedSubscriptionSettings": {
      "ConnectionString": "String",
      "DeadLetterExchangeName": "String",
      "ExchangeName": "String",
      "IsDurable": "Boolean",
      "QueueName": "String",
      "RoutingKey": "String"
    },
    "CandlesSharding": {
      "Shards": [
        {
          "Name": "String",
          "Pattern": "String"
        }
      ]
    },
    "CorporateActionsService": {
      "ApiKey": "String",
      "ServiceUrl": "String"
    },
    "Cqrs": {
      "ConnectionString": "String",
      "EnvironmentName": "String",
      "RetryDelay": "DateTime"
    },
    "Db": {
      "DataConnString": "String",
      "LogsConnString": "String",
      "StorageMode": "String"
    },
    "DefaultRateSettings": {
      "DefaultOnBehalfSettings": {
        "Commission": "Double",
        "CommissionAsset": "String",
        "LegalEntity": "String"
      },
      "DefaultOrderExecutionSettings": {
        "CommissionAsset": "String",
        "CommissionCap": "Integer",
        "CommissionFloor": "Double",
        "CommissionRate": "Double",
        "LegalEntity": "String"
      },
      "DefaultOvernightSwapSettings": {
        "FixRate": "Double",
        "RepoSurchargePercent": "Integer",
        "VariableRateBase": "String",
        "VariableRateQuote": "String"
      }
    },
    "InstanceId": "String",
    "LegacyAssetUpdatedRabbitPublisherSettings": {
      "ConnectionString": "String",
      "ExchangeName": "String"
    },
    "LegalEntityDefaults": {
      "DefaultLegalEntity": "String"
    },
    "MdmService": {
      "ApiKey": "String",
      "ServiceUrl": "String"
    },
    "RequestLoggerSettings": {
      "Enabled": "Boolean",
      "EnabledForGet": "Boolean",
      "MaxPartSize": "Integer"
    },
    "SettingsChangedRabbitMqSettings": {
      "ConnectionString": "String",
      "ExchangeName": "String"
    },
    "TradingInstrumentDefaults": {
      "CommissionCurrency": "String",
      "CommissionMax": "Integer",
      "CommissionMin": "Integer",
      "CommissionRate": "Integer",
      "DealMaxLimit": "Integer",
      "DealMinLimit": "Integer",
      "Delta": "Integer",
      "LeverageInit": "Integer",
      "LeverageMaintenance": "Integer",
      "PositionLimit": "Integer",
      "SwapLong": "Integer",
      "SwapShort": "Integer"
    },
    "UnderlyingChangedRabbitSubscriptionSettings": {
      "ConnectionString": "String",
      "DeadLetterExchangeName": "String",
      "ExchangeName": "String",
      "IsDurable": "Boolean",
      "QueueName": "String",
      "RoutingKey": "String"
    },
    "UseSerilog": "Boolean"
  },
  "MarginTradingAssetServiceClient": {
    "ApiKey": "String",
    "ServiceUrl": "String"
  },
  "serilog": {
    "Enrich": [
      "String"
    ],
    "minimumLevel": {
      "default": "String"
    },
    "Properties": {
      "Application": "String"
    },
    "Using": [
      "String"
    ],
    "writeTo": [
      {
        "Args": {
          "configure": [
            {
              "Args": {
                "outputTemplate": "String"
              },
              "Name": "String"
            }
          ]
        },
        "Name": "String"
      }
    ]
  },
  "TZ": "String"
}
```
<!-- MARKDOWN-AUTO-DOCS:END -->
