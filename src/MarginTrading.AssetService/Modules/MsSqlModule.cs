using System;
using Autofac;
using Lykke.Common.MsSql;
using Lykke.SettingsReader;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Settings.ServiceSettings;
using MarginTrading.AssetService.SqlRepositories;

namespace MarginTrading.AssetService.Modules
{
    public class MsSqlModule : Module
    {
        private readonly IReloadingManager<AssetServiceSettings> _settings;

        public MsSqlModule(IReloadingManager<AssetServiceSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            if (_settings.CurrentValue.Db.StorageMode == StorageMode.SqlServer)
            {
                if (string.IsNullOrEmpty(_settings.CurrentValue.Db.DataConnString))
                {
                    throw new Exception(
                        $"{nameof(_settings.CurrentValue.Db.DataConnString)} must have a value if StorageMode is SqlServer");
                }

                builder.RegisterMsSql(_settings.CurrentValue.Db.DataConnString,
                    connString => new AssetDbContext(connString, false),
                    dbConn => new AssetDbContext(dbConn));
            }
        }
    }
}