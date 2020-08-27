using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.AssetService.Extensions
{
    public static class ControllerExtensions
    {
        public static string TryGetCorrelationId(this ControllerBase controller)
        {
            controller.HttpContext.Request.Headers.TryGetValue("x-correlation-id", out var correlationIdValues);
            var correlationId = correlationIdValues.FirstOrDefault();
            return string.IsNullOrWhiteSpace(correlationId) ? Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture) : correlationId;
        }
    }
}