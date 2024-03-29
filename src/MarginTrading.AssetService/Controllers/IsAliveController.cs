﻿// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Net;
using Lykke.Common.Api.Contract.Responses;
using MarginTrading.AssetService.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.AssetService.Controllers
{
    // NOTE: See https://lykkex.atlassian.net/wiki/spaces/LKEWALLET/pages/35755585/Add+your+app+to+Monitoring
    [Route("api/[controller]")]
    public class IsAliveController : Controller
    {
        private readonly IHealthService _healthService;

        public IsAliveController(IHealthService healthService)
        {
            _healthService = healthService;
        }

        /// <summary>
        /// Checks service is alive
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IsAliveResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public IActionResult Get()
        {
            var healthViolationMessage = _healthService.GetHealthViolationMessage();
            if (healthViolationMessage != null)
            {
                return StatusCode(
                    (int)HttpStatusCode.InternalServerError,
                    ErrorResponse.Create($"Service is unhealthy: {healthViolationMessage}"));
            }

            // NOTE: Feel free to extend IsAliveResponse, to display job-specific indicators
            return Ok(new IsAliveResponse
            {
                Name = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationName,
                Version = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationVersion,
                Env = Environment.GetEnvironmentVariable("ENV_INFO"),
                IssueIndicators = _healthService.GetHealthIssues()
                    .Select(i => new IsAliveResponse.IssueIndicator
                    {
                        Type = i.Type,
                        Value = i.Value
                    })
            });
        }
    }
}
