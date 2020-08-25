using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.Audit;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
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
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<AuditContract>), (int)HttpStatusCode.OK)]
        public async Task<GetAuditLogsResponse> GetAuditTrailAsync([FromQuery]int? year, [FromQuery]int? month)
        {
            var result = await _auditService.GetAll(year, month);

            return new GetAuditLogsResponse
            {
                AuditLogs = result.Select(i => _convertService.Convert<IAuditModel,AuditContract>(i)).ToList()
            };
        }
    }
}