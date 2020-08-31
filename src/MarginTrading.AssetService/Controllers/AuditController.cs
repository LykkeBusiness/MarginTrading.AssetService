using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.Audit;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [ProducesResponseType(typeof(PaginatedResponseContract<AuditContract>), (int)HttpStatusCode.OK)]
        public async Task<PaginatedResponseContract<AuditContract>> GetAuditTrailAsync([FromQuery] GetAuditLogsRequest request, int? skip = null, int? take = null)
        {
            var filter = _convertService.Convert<GetAuditLogsRequest, AuditLogsFilterDto>(request);
            var result = await _auditService.GetAll(filter, skip, take);

            return new PaginatedResponseContract<AuditContract>(
                contents: result.Contents.Select(i => _convertService.Convert<IAuditModel, AuditContract>(i)).ToList(),
                start: result.Start,
                size: result.Size,
                totalSize: result.TotalSize
            );
        }
    }
}