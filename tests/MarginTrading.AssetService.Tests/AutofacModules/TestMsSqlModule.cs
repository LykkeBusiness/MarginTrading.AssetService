using System;
using Autofac;
using Lykke.Common.MsSql;
using MarginTrading.AssetService.SqlRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MarginTrading.AssetService.Tests.AutofacModules
{
    public class TestMsSqlModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var options = new DbContextOptionsBuilder<AssetDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb{Guid.NewGuid().ToString()}")
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            var contextFactory = new MsSqlContextFactory<AssetDbContext>(x => new AssetDbContext(options), options);

            builder
                .RegisterInstance(contextFactory)
                .AsSelf()
                .As<Lykke.Common.MsSql.IDbContextFactory<AssetDbContext>>()
                .As<ITransactionRunner>()
                .SingleInstance();
        }
    }
}