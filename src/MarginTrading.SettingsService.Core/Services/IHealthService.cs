// Copyright (c) 2019 Lykke Corp.

using System.Collections.Generic;
using MarginTrading.SettingsService.Core.Domain.Health;

namespace MarginTrading.SettingsService.Core.Services
{
    // NOTE: See https://lykkex.atlassian.net/wiki/spaces/LKEWALLET/pages/35755585/Add+your+app+to+Monitoring
    public interface IHealthService
    {
        string GetHealthViolationMessage();
        IEnumerable<HealthIssue> GetHealthIssues();
    }
}