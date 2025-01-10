### Rollback: <<<[ROLLBACK]_20241216115201_TestMigration3.sql>>>


## 2.30.0 - Nova 2. Delivery 48 (December 20, 2024)
### What's changed
* LT-5916: Update refit to 8.x version.
* LT-5882: Keep schema for appsettings.json up to date.


## 2.29.0 - Nova 2. Delivery 47 (November 15, 2024)
### What's changed
* LT-5859: Add new method: get assetpaircontract by id.
* LT-5815: Update messagepack to 2.x version.
* LT-5755: Add assembly load logger.
* LT-5729: Migrate to quorum queues.

### Deployment
In this release, all previously specified queues have been converted to quorum queues to enhance system reliability. The affected queues are:
- `lykke.assetservice.underlyingchanged`
- `lykke.assetservice.brokersettingschanged`

#### Automatic Conversion to Quorum Queues
The conversion to quorum queues will occur automatically upon service startup **if**:
* There are **no messages** in the existing queues.
* There are **no active** subscribers to the queues.

**Warning**: If messages or subscribers are present, the automatic conversion will fail. In such cases, please perform the following steps:
1. Run the previous version of the component associated with the queue.
1. Make sure all the messages are processed and the queue is empty.
1. Shut down the component associated with the queue.
1. Manually delete the existing classic queue from the RabbitMQ server.
1. Restart the component to allow it to create the quorum queue automatically.

#### Poison Queues
All the above is also applicable to the poison queues associated with the affected queues. Please ensure that the poison queues are also converted to quorum queues.

#### Disabling Mirroring Policies
Since quorum queues inherently provide data replication and reliability, server-side mirroring policies are no longer necessary for these queues. Please disable any existing mirroring policies applied to them to prevent redundant configurations and potential conflicts.

#### Environment and Instance Identifiers
Please note that the queue names may include environment-specific identifiers (e.g., dev, test, prod). Ensure you replace these placeholders with the actual environment names relevant to your deployment. The same applies to instance names embedded within the queue names (e.g., DefaultEnv, etc.).


## 2.28.0 - Nova 2. Delivery 46 (September 27, 2024)
### What's changed
* LT-5697: Discontinue product with date.
* LT-5586: Migrate to .net 8.


## 2.27.0 - Nova 2. Delivery 44 (August 15, 2024)
### What's changed
* LT-5527: Update rabbitmq broker library with new rabbitmq.client and templates.
* LT-5455: Trading disabled button.

### Deployment
Please ensure that the mirroring policy is configured on the RabbitMQ server side for the following queues:
- `lykke.assetservice.underlyingchanged`
- `lykke.assetservice.brokersettingschanged`

These queues require the mirroring policy to be enabled as part of our ongoing initiative to enhance system reliability. They are now classified as "no loss" queues, which necessitates proper configuration. The mirroring feature must be enabled on the RabbitMQ server side.

In some cases, you may encounter an error indicating that the server-side configuration of a queue differs from the client’s expected configuration. If this occurs, please delete the queue, allowing it to be automatically recreated by the client.

**Warning 1**: The "no loss" configuration is only valid if the mirroring policy is enabled on the server side.

**Warning 2**: Please delete the following poison queues if they exist before running the new version of a component: 
- `lykke.assetservice.brokersettingschanged-{...}-poison`
- `lykke.assetservice.underlyingchanged-{...}-poison`.

Please be aware that the provided queue names may include environment-specific identifiers (e.g., dev, test, prod). Be sure to replace these with the actual environment name in use. The same applies to instance names embedded within the queue names (e.g., DefaultEnv, etc.).


## 2.26.0 - Nova 2. Delivery 41 (March 29, 2024)
### What's changed
* LT-5307: Use new type for assetid.


## 2.25.1 - Nova 2. Delivery 40. Hotfix 5 (May 14, 2024)
### What's changed
* LT-5491: Holidays notifications wrong time


## 2.25.0 - Nova 2. Delivery 40 (February 28, 2024)
### What's changed
* LT-5284: Step: deprecated packages validation is failed.
* LT-5215: Update lykke.httpclientgenerator to 5.6.2.


## 2.24.0 - Nova 2. Delivery 39 (January 26, 2024)
### What's changed
* LT-5166: Asset service: add history of releases into changelog.md for github.
* LT-4945: Wrong behaviour for a request get /api/assetpairs/by-pages.


## 2.23.0 - Nova 2. Delivery 38 (December 08, 2023)
### What's changed
* LT-5120: Add changelog.md file.


## 2.22.2 - Nova 2. Delivery 37 (October 17, 2023)
### What's changed
* LT-5042: Add technical 871m migration endpoint.
* LT-5024: Introduce asset type cache.
* LT-4973: Update messaging libraries.
* LT-4970: Product not frozen after the start of a product expiry coa.
* LT-4967: Product not discontinued "could not discontinue product with id:cfd_us_bbby_bbva, because of error:doesnotexist".

## 2.21.2 - Nova 2. Delivery 36
## What's changed
* LT-4943: Warnings when we add or delete a broker.
* LT-4935: Add maxpositionnotional new column.
* LT-4891: Update nugets.

**Full change log**: https://github.com/lykkebusiness/margintrading.assetservice/compare/v2.20.2...v2.21.2


## 2.20.2 - Nova 2. Delivery 35
## What's changed
* LT-4766: Fix getprecision method.
* LT-4781: Filter product by mds code (not full match).
* LT-4868: Improve getprecision method.

**Full Changelog**: https://github.com/LykkeBusiness/MarginTrading.AssetService/compare/v2.19.3...v2.20.2

## v2.19.19 - Nova 2. Delivery 34. Hotfix 4
## What's changed
* LT-4821: Add direct dependency on Lykke.Snow.Common package

**Full Changelog**: https://github.com/LykkeBusiness/MarginTrading.AssetService/compare/v2.19.4...v2.19.19

## v2.19.4 - Nova 2. Delivery 34. Hotfix 3
## What's changed
* LT-4793: Add direct dependency on Lykke.Snow.Common package

**Full Changelog**: https://github.com/LykkeBusiness/MarginTrading.AssetService/compare/v2.19.3...v2.19.4

## v2.19.3 - Nova 2. Delivery 34
## What's Changed

- LT-4684: Update Lykke.Snow.Mdm.Contracts package
- LT-4724: Delete MinOrderEntryInterval and Tags from Product
- LT-4754: Use new ValidateSettings from common startup

### Deployment
- Make sure to create a backup for dbo.Products table since the data migration will cut the time portion out of the Products.StartDate column.

**Full Changelog**: https://github.com/LykkeBusiness/MarginTrading.AssetService/compare/v2.18.2...v2.19.3

## v2.18.2 - Nova 2. Delivery 33
## What's changed
* LT-4653: Share contract size with `AssetPairContract`.
* LT-4636: Move precision logic from donut.
* LT-4612: Make `TradingDayInfo` object consistent.
* LT-4573: Add `ExcludeSpreadFromProductCosts` and expose to `ClientProfile`.
* LT-4555: Add validation (contract size) for already started product.


**Full change log**: https://github.com/lykkebusiness/margintrading.assetservice/compare/v2.17.4...v2.18.2

## v2.17.4 - Nova 2. Delivery 32
## What's changed
* LT-4539: Put back host restart attempts.
* LT-4397: Do not let the host keep running if startupmanager failed to start.
* LT-4225: Validateskipandtake implementation replace.


**Full change log**: https://github.com/lykkebusiness/margintrading.assetservice/compare/v2.16.13...v2.17.4

## v2.16.13 - Nova 2. Delivery 31
## What's changed
* LT-4372: Update messaging libraries to fix verify endpoints.

**Full Changelog**: https://github.com/LykkeBusiness/MarginTrading.AssetService/compare/v2.16.3...v2.16.13

## v2.16.3 - Nova 2. Delivery 28. Hotfix 3
* LT-4321: Upgrade LykkeBiz.Logs.Serilog to 3.3.1

## v2.16.1 - Nova 2. Delivery 28
## What's Changed
* LT-3721: NET 6 migration

### Deployment
* NET 6 runtime is required
* Dockerfile is updated to use native Microsoft images (see [DockerHub](https://hub.docker.com/_/microsoft-dotnet-runtime/))

**Full Changelog**: https://github.com/LykkeBusiness/MarginTrading.AssetService/compare/v2.15.3...v2.16.1

## v2.15.3 - Nova 2. Delivery 26
* LT-4124: Fix an issue with GetAllAsync (IsDiscontinued = false, all products)
* LT-4105: api/tradingInstruments/unavailable not working under refit

## v2.15.0 - Nova 2. Delivery 25
## What's Changed
* fix(LT-3994): updated audit implementation by @tarurar in https://github.com/LykkeBusiness/MarginTrading.AssetService/pull/202
* fix(LT-3963): removing leading and trailing white spaces when adding … by @tarurar in https://github.com/LykkeBusiness/MarginTrading.AssetService/pull/201


**Full Changelog**: https://github.com/LykkeBusiness/MarginTrading.AssetService/compare/v2.14.0...v2.15.0

## v2.14.0 - Nova 2. Delivery 23
* LT-3867: Add IsTradingDisabled flag & edit api

### Deployment
* New API added in Asset service `/api/products/{productId}/disable-trading`

## v2.13.6 - Nova 2. Delivery 22. Hotfix 1
* LT-3864: Add migration script for HedgeCost

### Deployment
* Stop services: Asset, Commission, Bookkeeper, MT Core, Donut
* Execute script `src/MarginTrading.AssetService.SqlRepositories/Migrations/Scripts/hedge_cost_migration.sql` on the broker's database (the `dbo.Products` table will be affected)
* Start services
* Manually check that updated values (HedgeCost on products) are correct

## v2.13.4 - Nova 2. Delivery 20. Hotfix 7
* LT-3864: Add migration script for HedgeCost

### Deployment
* Stop services: Asset, Commission, Bookkeeper, MT Core, Donut
* Execute script `src/MarginTrading.AssetService.SqlRepositories/Migrations/Scripts/hedge_cost_migration.sql` on the broker's database (the `dbo.Products` table will be affected)
* Start services
* Manually check that updated values (HedgeCost on products) are correct

## v2.13.3 - Nova 2. Delivery 18. Hotfix 14
* LT-3864: Add migration script for HedgeCost

### Deployment
* Stop services: Asset, Commission, Bookkeeper, MT Core, Donut
* Execute script `src/MarginTrading.AssetService.SqlRepositories/Migrations/Scripts/hedge_cost_migration.sql` on the broker's database (the `dbo.Products` table will be affected)
* Start services
* Manually check that updated values (HedgeCost on products) are correct

## v2.13.1 - Nova 2. Delivery 22
* LT-3785: Extend Asset contract adding EnforceMargin & Margin fields
* LT-3783: Eligible CoA failed to discontinue Product

## v2.12.0 - Nova 2. Delivery 21
* LT-3717: NOVA security threats

## v2.11.7 - Nova 2. Delivery 20
* LT-3697: CorrelationId cleanup

## v2.11.5 - Nova 2. Delivery 18. Hotfix 10
* LT-3702: Shift market schedule to UTC within the LegacyAssetService 
* LT-3689: Add diagnostic logging for client profile settings cache

### Deployment

- Version 10.2.3 of nuget package `Lykke.MarginTrading.AssetService.Contracts` has been published.

## v2.11.6 - Nova 2. Delivery 19. Hotfix 1
* LT-3702: Shift market schedule to UTC within the LegacyAssetService 
* LT-3689: Add diagnostic logging for client profile settings cache

### Deployment

- Version 10.2.3 of nuget package `Lykke.MarginTrading.AssetService.Contracts` has been published.

## v2.11.3 - Nova 2. Delivery 19
* LT-3635: Unable to create underlying
* LT-3670: Add endpoint to get not started legacy asset

### Deployment
* New version 10.0.0 of nuget package `Lykke.MarginTrading.AssetService.Contracts` has been published.

## v2.11.2 - Nova 2. Delivery 18.
### Tasks

* LT-3559: Retract time intervals for products retrieving

## v2.11.1 - Nova 2. Delivery 17.
* LT-3543: Add api for checking unavailable products
* LT-3535: Use one event for Asset Suspend / Unsuspend events
* LT-3491: Update dividend percent validations

## v2.10.10 - Nova 2. Delivery 16.
* LT-3464: Platform holiday not displayed on instrument status

## v2.10.9 - Nova 2. Delivery 15.
### Bugfixes

* LT-3419: Update trading instrument contract to update decimal fields

## v2.10.8 - Nova 2. Delivery 14. Hotfix 1
* LT-3445: ISIN Validation does not allow referential import

## v2.10.5 - Nova 2. Delivery 14.
### Tasks
* LT-3226: Add validations for ISIN

## v2.10.3 - Nova 2. Delivery 13.
### Tasks
* LT-3279: Automatic asset creation with future start date
* LT-3265: Publish update event when default client profile becomes not default
* LT-3201: Update snow.common and add new error codes
* LT-3295: Currency can be deleted even if it relates to product (support)

## v2.10.1 - Nova 2. Delivery 12.
### Tasks

* LT-3120: Update legacy asset model to break backward compatibility
* LT-3145: Remove OvernightSwapRateContract.FixRate in asset service contracts

### Deployment

* Removed configuration section DefaultRateSettings.DefaultOrderExecutionSettings
* The configuration key DefaultOvernightSwapSettings.FixRate has been removed

## v2.9.2 - Delivery 11
### Tasks

* LT-3222: Add IsDiscontinued to paginated Products request

## v2.8.1 - Bugfix
### Tasks

* LT-3186: [Asset] Fix ProductsCount api

## v2.8.0 - Nova 2. Delivery 9.
### Tasks

* LT-3130: Add TZ = Europe/Berlin for all our services
* LT-3156: Revert interest rates mapping (C&C calculations)

## v2.7.1 - Nova 2. Delivery 8.
### Tasks

* LT-3104: Update all products when mds code is changed
* LT-3092: Update profiles feature
* LT-3089: there is no update for "NextTradingDayStart" for POST ​/api​/scheduleSettings​/markets-info request in case multi session schedule
* LT-3068: Map Half working days for legacy asset api

## v2.6.5 - Nova 2. Delivery 7. Hotfix 2.
### Tasks

* Updated Cronut.Dto package version

## v2.6.0 - Nova 2. Delivery 7.
### Tasks

* LT-2748: Recognise new format for open and close hours
* LT-2835: Get weekends from Broker Settings
* LT-2964: Need to remove uniqueness check for Product name field
* LT-2977: Adjust Start date implementation
* LT-3012: Update search legacy assets logic

## v2.5.1 - Hotfix for delivery 6.
### Tasks

* LT-2989: PRODUCTION CRITICAL 1D - Regression - PDL - Interest rates not correctly managed

### Deployment

Added new settings to MarginTradingAssetService section, where the list of asset types for which interest rates should be 0 needs to be specified:

```json
"AssetTypesWithZeroInterestRates": [
"YourAssetTypeCouldBeHere"
]
```

## v2.3.5 - Hotfix for delivery 4.
### Tasks

* LT-2989: PRODUCTION CRITICAL 1D - Regression - PDL - Interest rates not correctly managed

### Deployment

Added new settings to MarginTradingAssetService section, where the list of asset types for which interest rates should be 0 needs to be specified:

```json
"AssetTypesWithZeroInterestRates": [
"YourAssetTypeCouldBeHere"
]
```

## v2.5.0 - Nova 2. Delivery 6.
### Tasks

* LT-2917: Error after creation a product with ID the same as currency ID
* LT-2901: Move search logic of products by requirest params from Donut to Asset Service (search in Legacy Asset cache)

## v2.4.0 - Nova 2. Delivery 5.
### Tasks

* LT-2904: EOD failed due to FX fixing mismatch
* LT-2817: Investigate situation with concurrency when we update statuses for suspended
* LT-2875: Wrong trading instrument status for new instrument
* LT-2753: Unhardcode EUR
* LT-2759: Add UnderlyingCategory to AssetType
* LT-2791: Fetching products with POST request instead of GET
* LT-2794: Fix currency mapping in legacy api
* LT-2827: Populate Available property of Asset
* LT-2834: Add constraint to product Id
* LT-2839: Set LegacyAsset Underlying Ric from Product property
* LT-2857: Add isBusinessDay in platform info
* LT-2864: Add new fields to the product
* LT-2914: Update asset service to include MarketName in ScheduleSettingsContract

### Deployment

* Backoffice v1.4.0, MT-Core v2.3.2 and Commissions Service v2.30 should be deployed

* Run SQL script:
```sql
UPDATE dbo.Products
SET IsSuspended = 1
WHERE StartDate >getdate()
```

* Add to **MarginTradingAssetService** section **BrokerSettingsChangedSubscriptionSettings**:
```json
"BrokerSettingsChangedSubscriptionSettings": {
"ConnectionString": "{rabbit-mq-conn-string}",
"ExchangeName": "dev.MdmService.events.exchange",
"RoutingKey": "BrokerSettingsChangedEvent",
"QueueName": "lykke.assetservice.brokersettingschanged",
"IsDurable": false,
"DeadLetterExchangeName": "dev.MdmService.events.exchange.dlx"
}
```

* Hedge cost values should migrated from underlyings to the corresponding products (if not done yet). It is possible to do that via SQL, if you will be able to connect global and broker DBs (for example using https://docs.microsoft.com/ru-ru/sql/relational-databases/linked-servers/linked-servers-database-engine?view=sql-server-ver15) or you could use underlyings/products export/import feature.

```sql
update p
set p.HedgeCost = u.HedgeCost
from dbo.Products p
inner join mdm.Underlyings u on p.UnderlyingMdsCode = u.MdsCode
```

## v2.3.1 - Bugfixes
### Tasks

* LT-2810: Update HttpClientGenerator lib to get HTTP 400 details
* LT-2802: Forbid isDefault update for non default id

## v2.3.0 - Nova 2. Delivery 4.
### Tasks

* LT-2700: Filter audit trail by multiple data types
* LT-2707: Add start date to product
* LT-2717: Improve asset adding workflow
* LT-2788: Fix ProductChangedEvent on discontinue

## v2.2.2 - Nova 2. New ref data model usage.
### Main changes

* All "old" APIs (Assets, Asset Pairs, Schedules, etc.) will now return data from the new data model.
* New "Legacy asset" API, that returns the data in the same format as it is stored currently in Chest, but data comes from a new data model.

### Tasks

*LT-2538: Implemented unit tests for tick formula validations
*LT-2575: Introduce RabbitMq events for entities modifications
*LT-2576: Use new entities in old APIs
*LT-2590: Implement filter by underlyings mds codes list / product IDs
*LT-2593: Fix fk on product - market settings
*LT-2597: Implement product freeze in asset service
*LT-2608: Implement APIs for bulk edit for products
*LT-2622: Update product in asset service
*LT-2641: Implement mapping from new ref data to existing donut Asset
*LT-2643: Validate product categories in asset service
*LT-2644: Fix refit contract in asset service
*LT-2656: Clean up Asset Service
*LT-2657: Add new product state to freeze endpoint response
*LT-2666: Add endpoint to discontinue products
*LT-2669: Order AuditTrail records by Timestamp descending
*LT-2675: Add endpoint to check if there are products for underlying
*LT-2683: Add validation after applying timezone to open and close
*LT-2692: GET ​/api​/client-profile-settings​/will-violate-regulation-constraint works incorrectly for Regulations not tied to a broker.

### Deployment

* Add new settings to MarginTradingAssetService section

```json

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
    }

```

* Rabbit Mq changes: after the deployment of the corresponding services, a log of binding will be created in the exchange dev.SettingsService.events.exchange

* After data migration execute SQL script that will update product flags: src/MarginTrading.AssetService/Scripts/MigrateAssetPairStatusesToProducts.sql

## v2.1.1 - New APIs required for the new ref file model.
### Tasks

* LT-2471: API for products management
* LT-2478: Adjust Audit trail to comply to the newer specs
* LT-2484: Hide EF warnings for decimal precision
* LT-2509: Implement Markets
* LT-2521: Impement Currencies API
* LT-2524: Change ClientProfiles and AssetTypes to use Name as Id
* LT-2533: Implement TickFormula API
* LT-2535: Create category api in asset service
* LT-2540: Add delete constraint for MarketSettings
* LT-2556: Sync implementation with the specs (products and currencies)
* LT-2560: Change MarketSettings and Margin Validations
* LT-2563: Extend ClientProfileSettings with RegulatoryProfileId and RegulatoryTypeId
* LT-2580: Extend MarketSettings holidays validation
* LT-2542: Link Products with all related entities

### RabbitMQ changes.

New bindings to the queue dev.SettingsService.events.exchange 
* to queue dev.Chest.queue.SettingsService.events.CurrencyChangedEvent.projections with routingKey CurrencyChangedEvent
* to queue dev.Chest.queue.SettingsService.events.ProductCategoryChangedEvent.projections with routing key ProductCategoryChangedEvent

## v2.0.1 - New version of service
### Description

Service was renamed from MarginTrading.SettingsService to MarginTrading.AssetService and consolidated all objects, related to asset management (including rates and profiles).

### Tasks

* LT-2360: [ARCH] Rename Settings Service to Asset Service
* LT-2399: Add RateSettings Controller and update service contract
* LT-2447: Client Profiles and Asset Types management API and Storage

### Deployment

* Move CommissionService.DefaultRateSettings to MarginTradingAssetService.DefaultRateSettings 
Example:
```json
   "DefaultRateSettings":{
      "DefaultOrderExecutionSettings":{
         "CommissionCap":69,
         "CommissionFloor":9.95,
         "CommissionRate":0.001,
         "CommissionAsset":"EUR",
         "LegalEntity":"Default"
      },
      "DefaultOvernightSwapSettings":{
         "RepoSurchargePercent":0,
         "FixRate":0.035,
         "VariableRateBase":"",
         "VariableRateQuote":""
      },
      "DefaultOnBehalfSettings":{
         "Commission":14.95,
         "CommissionAsset":"EUR",
         "LegalEntity":"Default"
      }
   }
```

### :warning: Dependency :warning:

Trading Core should be updated to v2.0.0 before this version of Asset Service is deployed because new values were added to RAbbitMQ messages and without the update, Trading Core will stop consuming messages.
