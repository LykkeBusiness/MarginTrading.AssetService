// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Autofac;
using BookKeeper.Client.Workflow.Events;
using Common.Log;
using Lykke.Cqrs;
using Lykke.Cqrs.Configuration;
using Lykke.Cqrs.Configuration.BoundedContext;
using Lykke.Cqrs.Configuration.Routing;
using Lykke.Cqrs.Configuration.Saga;
using Lykke.Cqrs.Middleware.Logging;
using Lykke.Messaging;
using Lykke.Messaging.Contract;
using Lykke.Messaging.RabbitMq;
using Lykke.Messaging.Serialization;
using Lykke.Snow.Mdm.Contracts.Models.Events;
using MarginTrading.AssetService.Contracts.AssetPair;
using MarginTrading.AssetService.Contracts.AssetTypes;
using MarginTrading.AssetService.Contracts.ClientProfiles;
using MarginTrading.AssetService.Contracts.ClientProfileSettings;
using MarginTrading.AssetService.Contracts.Currencies;
using MarginTrading.AssetService.Contracts.MarketSettings;
using MarginTrading.AssetService.Contracts.ProductCategories;
using MarginTrading.AssetService.Contracts.Products;
using MarginTrading.AssetService.Contracts.TickFormula;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Settings;
using MarginTrading.AssetService.Settings.ServiceSettings;
using MarginTrading.AssetService.Workflow.AssetPairFlags;
using MarginTrading.AssetService.Workflow.ClientProfiles;
using MarginTrading.AssetService.Workflow.ClientProfileSettings;
using MarginTrading.AssetService.Workflow.Currencies;
using MarginTrading.AssetService.Workflow.MarketSettings;
using MarginTrading.AssetService.Workflow.ProductCategories;
using MarginTrading.AssetService.Workflow.Products;
using MarginTrading.AssetService.Workflow.TickFormulas;

namespace MarginTrading.AssetService.Modules
{
    internal class CqrsModule : Module
    {
        private const string DefaultRoute = "self";
        private const string DefaultPipeline = "commands";
        private const string DefaultEventPipeline = "events";
        private readonly CqrsSettings _settings;
        private readonly ILog _log;
        private readonly string _instanceId;
        private readonly long _defaultRetryDelayMs;
        private readonly CqrsContextNamesSettings _contextNames;

        public CqrsModule(CqrsSettings settings, ILog log, string instanceId)
        {
            _settings = settings;
            _log = log;
            _instanceId = instanceId;
            _defaultRetryDelayMs = (long) _settings.RetryDelay.TotalMilliseconds;
            _contextNames = _settings.ContextNames;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context => new AutofacDependencyResolver(context)).As<IDependencyResolver>()
                .SingleInstance();
            
            builder.RegisterInstance(_contextNames).AsSelf().SingleInstance();

            var rabbitMqSettings = new RabbitMQ.Client.ConnectionFactory
            {
                Uri = new Uri(_settings.ConnectionString, UriKind.Absolute)
            };
            var messagingEngine = new MessagingEngine(_log,
                new TransportResolver(new Dictionary<string, TransportInfo>
                {
                    {
                        "RabbitMq",
                        new TransportInfo(rabbitMqSettings.Endpoint.ToString(), rabbitMqSettings.UserName,
                            rabbitMqSettings.Password, "None", "RabbitMq")
                    }
                }),
                new RabbitMqTransportFactory());

            // Sagas & command handlers
            builder.RegisterAssemblyTypes(GetType().Assembly)
                .Where(t => t.Name.EndsWith("Saga") || t.Name.EndsWith("CommandsHandler") ||
                            t.Name.EndsWith("Projection"))
                .AsSelf();

            builder.Register(ctx => CreateEngine(ctx, messagingEngine))
                .As<ICqrsEngine>()
                .SingleInstance()
                .AutoActivate();
        }

        private CqrsEngine CreateEngine(IComponentContext ctx, IMessagingEngine messagingEngine)
        {
            var rabbitMqConventionEndpointResolver = new RabbitMqConventionEndpointResolver(
                "RabbitMq",
                SerializationFormat.MessagePack,
                environment: _settings.EnvironmentName);

            var engine = new CqrsEngine(
                _log,
                ctx.Resolve<IDependencyResolver>(),
                messagingEngine,
                new DefaultEndpointProvider(),
                true,
                Register.DefaultEndpointResolver(rabbitMqConventionEndpointResolver),
                RegisterDefaultRouting(),
                RegisterStartProductsSaga(),
                RegisterContext(),
                Register.CommandInterceptors(new DefaultCommandLoggingInterceptor(_log)),
                Register.EventInterceptors(new DefaultEventLoggingInterceptor(_log)));

            engine.StartPublishers();

            return engine;
        }

        private IRegistration RegisterContext()
        {
            var contextRegistration = Register.BoundedContext(_contextNames.AssetService)
                .FailedCommandRetryDelay(_defaultRetryDelayMs)
                .ProcessingOptions(DefaultRoute).MultiThreaded(8).QueueCapacity(1024);
            RegisterAssetPairFlagsCommandHandler(contextRegistration);
            RegisterEventPublishing(contextRegistration);
            RegisterAssetServiceProjections(contextRegistration);
            RegisterStartProductsHandler(contextRegistration);

            return contextRegistration;
        }

        private PublishingCommandsDescriptor<IDefaultRoutingRegistration> RegisterDefaultRouting()
        {
            return Register.DefaultRouting
                .PublishingCommands(
                )
                .To(_contextNames.AssetService)
                .With(DefaultPipeline);
        }

        private IRegistration RegisterStartProductsSaga()
        {
            var sagaRegistration = RegisterSaga<StartProductsSaga>();

            sagaRegistration
                .ListeningEvents(typeof(EodProcessFinishedEvent))
                .From(_settings.ContextNames.BookKeeper)
                .On(nameof(EodProcessFinishedEvent))
                .PublishingCommands(typeof(StartProductCommand))
                .To(_contextNames.AssetService)
                .With(DefaultPipeline);

            return sagaRegistration;
        }

        private static void RegisterStartProductsHandler(
            ProcessingOptionsDescriptor<IBoundedContextRegistration> contextRegistration)
        {
            contextRegistration
                .ListeningCommands(typeof(StartProductCommand))
                .On(DefaultRoute)
                .WithCommandsHandler<StartProductsCommandsHandler>()
                .PublishingEvents(typeof(ProductChangedEvent))
                .With(DefaultEventPipeline);
        }

        private static void RegisterAssetPairFlagsCommandHandler(
            ProcessingOptionsDescriptor<IBoundedContextRegistration> contextRegistration)
        {
            contextRegistration
                .ListeningCommands(
                    typeof(SuspendAssetPairCommand),
                    typeof(UnsuspendAssetPairCommand)
                )
                .On(DefaultRoute)
                .WithCommandsHandler<AssetPairFlagsCommandsHandler>()
                .PublishingEvents(
                    typeof(AssetPairChangedEvent)
                )
                .With(DefaultPipeline);
        }

        private ISagaRegistration RegisterSaga<TSaga>()
        {
            return Register.Saga<TSaga>($"{_contextNames.AssetService}.{typeof(TSaga).Name}");
        }

        private static void RegisterEventPublishing(
            ProcessingOptionsDescriptor<IBoundedContextRegistration> contextRegistration)
        {
            contextRegistration.PublishingEvents(typeof(MarketSettingsChangedEvent),
                    typeof(ProductCategoryChangedEvent),
                    typeof(CurrencyChangedEvent),
                    typeof(ProductChangedEvent),
                    typeof(ClientProfileChangedEvent),
                    typeof(ClientProfileSettingsChangedEvent),
                    typeof(TickFormulaChangedEvent),
                    typeof(AssetTypeChangedEvent)
                )
                .With(DefaultEventPipeline);
        }

        private void RegisterAssetServiceProjections(
            ProcessingOptionsDescriptor<IBoundedContextRegistration> contextRegistration)
        {
            contextRegistration.ListeningEvents(typeof(MarketSettingsChangedEvent))
                .From(_settings.ContextNames.AssetService)
                .On($"{nameof(MarketSettingsChangedEvent)}{_instanceId}")
                .WithProjection(typeof(MarketSettingsChangedProjection), _settings.ContextNames.AssetService);

            contextRegistration.ListeningEvents(typeof(ProductCategoryChangedEvent))
                .From(_settings.ContextNames.AssetService)
                .On($"{nameof(ProductCategoryChangedEvent)}{_instanceId}")
                .WithProjection(typeof(ProductCategoryChangedProjection), _settings.ContextNames.AssetService);

            contextRegistration.ListeningEvents(typeof(ProductChangedEvent))
                .From(_settings.ContextNames.AssetService)
                .On($"{nameof(ProductChangedEvent)}{_instanceId}").WithProjection(typeof(ProductChangedProjection),
                    _settings.ContextNames.AssetService);

            contextRegistration.ListeningEvents(typeof(ClientProfileChangedEvent))
                .From(_settings.ContextNames.AssetService)
                .On($"{nameof(ClientProfileChangedEvent)}{_instanceId}")
                .WithProjection(typeof(ClientProfileChangedProjection), _settings.ContextNames.AssetService);

            contextRegistration.ListeningEvents(typeof(ClientProfileSettingsChangedEvent))
                .From(_settings.ContextNames.AssetService)
                .On($"{nameof(ClientProfileSettingsChangedEvent)}{_instanceId}")
                .WithProjection(typeof(ClientProfileSettingsChangedProjection), _settings.ContextNames.AssetService);

            contextRegistration.ListeningEvents(typeof(TickFormulaChangedEvent))
                .From(_settings.ContextNames.AssetService)
                .On($"{nameof(TickFormulaChangedEvent)}{_instanceId}")
                .WithProjection(typeof(TickFormulaChangedProjection), _settings.ContextNames.AssetService);

            contextRegistration.ListeningEvents(typeof(CurrencyChangedEvent))
                .From(_settings.ContextNames.AssetService)
                .On($"{nameof(CurrencyChangedEvent)}{_instanceId}").WithProjection(typeof(CurrencyChangedProjection),
                    _settings.ContextNames.AssetService);
        }
    }
}