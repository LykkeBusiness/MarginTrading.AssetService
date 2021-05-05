
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

```json
{
  "MarginTradingAssetService": {
    "MdmService": {
      "ApiKey": "secret key",
      "ServiceUrl": "http://mdm.mt.svc.cluster.local"
    },
    "BrokerId": "Consors",
    "InstanceId": "mtasset-1",
    "LegacyAssetUpdatedRabbitPublisherSettings": {
      "ConnectionString": "amqp://user:password@rabbit-mq-url:5672",
      "ExchangeName": "lykke.donut.assetupdates"
    },
    "UnderlyingChangedRabbitSubscriptionSettings": {
      "ConnectionString": "amqp://user:password@global-rabbit-mq-url:5672",
      "ExchangeName": "dev.MdmService.events.exchange",
      "RoutingKey": "UnderlyingChangedEvent",
      "QueueName": "lykke.assetservice.underlyingchanged",
      "IsDurable": false,
      "DeadLetterExchangeName": "dev.MdmService.events.exchange.dlx"
    },
    "CandlesSharding": {
      "Shards": [
        {
          "Name": "facebook",
          "Pattern": "facebook"
        }
      ]
    },
    "Db": {
      "StorageMode": "SqlServer",
      "DataConnString": "data connection string",
      "LogsConnString": "logs connection string"
    },
    "SettingsChangedRabbitMqSettings": {
      "ConnectionString": "amqp://login:pwd@rabbit-mt.mt.svc.cluster.local:5672",
      "ExchangeName": "MtCoreSettingsChanged"
    },
    "TradingInstrumentDefaults": {
      "LeverageInit": 1,
      "LeverageMaintenance": 1,
      "SwapLong": 1,
      "SwapShort": 1,
      "Delta": 1,
      "DealMinLimit": 1,
      "DealMaxLimit": 1,
      "PositionLimit": 1,
      "CommissionRate": 1,
      "CommissionMin": 1,
      "CommissionMax": 1,
      "CommissionCurrency": "USD"
    },
    "LegalEntityDefaults": {
      "DefaultLegalEntity": "Default"
    },
    "RequestLoggerSettings": {
      "Enabled": true,
      "MaxPartSize": 2048,
      "EnabledForGet": false
    },
    "Cqrs": {
      "ConnectionString": "amqp://login:pwd@rabbit-mt.mt.svc.cluster.local:5672",
      "RetryDelay": "00:00:02",
      "EnvironmentName": "env name"
    },
    "ChaosKitty": {
      "StateOfChaos": 0
    },
    "UseSerilog": false,
    "DefaultRateSettings": {
      "DefaultOrderExecutionSettings": {
        "CommissionCap": 69,
        "CommissionFloor": 9.95,
        "CommissionRate": 0.001,
        "CommissionAsset": "EUR",
        "LegalEntity": "Default"
      },
      "DefaultOvernightSwapSettings": {
        "RepoSurchargePercent": 0,
        "VariableRateBase": "",
        "VariableRateQuote": ""
      },
      "DefaultOnBehalfSettings": {
        "Commission": 14.95,
        "CommissionAsset": "EUR",
        "LegalEntity": "Default"
      }
    }
  },
  "MarginTradingAssetServiceClient": {
  "ServiceUrl": "http://mt-asset-service.mt.svc.cluster.local",
  "ApiKey": "secret key"
   }
}
```
