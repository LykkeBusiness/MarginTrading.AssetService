// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Threading.Tasks;
using Common;
using JetBrains.Annotations;
using MarginTrading.AssetService.Core.Helpers;
using MarginTrading.AssetService.Settings.ServiceSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MarginTrading.AssetService.Middleware
{
    [UsedImplicitly]
    public class RequestsLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RequestLoggerSettings _settings;
        private readonly ILogger<RequestsLoggingMiddleware> _logger;

        private const int MaxStorageFieldLength = 2000;

        public RequestsLoggingMiddleware(RequestDelegate next, RequestLoggerSettings settings, ILogger<RequestsLoggingMiddleware> logger)
        {
            _next = next;
            _settings = settings;
            _logger = logger;
        }

        [UsedImplicitly]
        public async Task Invoke(HttpContext context)
        {
            var requestContext =
                $"Request path: {context?.Request?.Path}, {Environment.NewLine}Method: {context?.Request?.Method}";
            try
            {
                if (_settings.Enabled && (_settings.EnabledForGet || context.Request.Method.ToUpper() != "GET"))
                {
                    var reqBodyStream = new MemoryStream();
                    var originalRequestBody = new MemoryStream();

                    await context.Request.Body.CopyToAsync(reqBodyStream);
                    reqBodyStream.Seek(0, SeekOrigin.Begin);
                    await reqBodyStream.CopyToAsync(originalRequestBody);
                    reqBodyStream.Seek(0, SeekOrigin.Begin);
                    context.Request.Body = reqBodyStream;

                    using (originalRequestBody)
                    {
                        var body = await StreamHelpers.GetStreamPart(originalRequestBody, _settings.MaxPartSize);
                        var headers = context.Request.Headers.ToJson();
                        var info = $"Body: {body} {Environment.NewLine}Headers:{headers}";
                        if (info.Length > MaxStorageFieldLength)
                        {
                            info = info.Substring(0, MaxStorageFieldLength);
                        }

                        _logger.LogInformation("MIDDLEWARE: {Info}, Context: {Context}", info, requestContext);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MIDDLEWARE: Failed to log request, Context: {Context}", requestContext);
            }
            finally
            {
                await _next.Invoke(context);
            }
        }
    }
}
