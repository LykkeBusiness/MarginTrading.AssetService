using JetBrains.Annotations;

namespace MarginTrading.SettingsService.Settings.ServiceSettings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class LykkeServiceSettings
    {
        public DbSettings Db { get; set; }
    }
}
