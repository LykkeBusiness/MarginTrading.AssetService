﻿using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Lykke.Contracts.Responses;
using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.Audit;
using MarginTrading.AssetService.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuditDataType = MarginTrading.AssetService.Core.Domain.AuditDataType;

namespace MarginTrading.AssetService.Controllers
{
    [Route("api/audit")]
    [Authorize]
    [ApiController]
    public class AuditController : ControllerBase, IAuditApi
    {
        private readonly IAuditService _auditService;
        private readonly IConvertService _convertService;

        public AuditController(IAuditService auditService, IConvertService convertService)
        {
            _auditService = auditService;
            _convertService = convertService;
        }

        /// <summary>
        /// Get audit logs
        /// </summary>
        /// <param name="request"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponse<AuditContract>), (int)HttpStatusCode.OK)]
        public async Task<PaginatedResponse<AuditContract>> GetAuditTrailAsync([FromQuery] GetAuditLogsRequest request, int? skip = null, int? take = null)
        {
            var filter = _convertService.Convert<GetAuditLogsRequest, AuditTrailFilter<AuditDataType>>(request);
            var result = await _auditService.GetAll(filter, skip, take);

            return new PaginatedResponse<AuditContract>(
                contents: result.Contents.Select(i => _convertService.Convert<IAuditModel<AuditDataType>, AuditContract>(i)).ToList(),
                start: result.Start,
                size: result.Size,
                totalSize: result.TotalSize
            );
        }
    }
}