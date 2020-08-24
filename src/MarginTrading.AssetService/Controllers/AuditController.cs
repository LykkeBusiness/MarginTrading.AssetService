using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.Audit;
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
        private readonly IMapper _mapper;

        public AuditController(IAuditService auditService, IMapper mapper)
        {
            _auditService = auditService;
            _mapper = mapper;
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
                AuditLogs = _mapper.Map<IReadOnlyList<AuditContract>>(result)
            };
        }
    }
}