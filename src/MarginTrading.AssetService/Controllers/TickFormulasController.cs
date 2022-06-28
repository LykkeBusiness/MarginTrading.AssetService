using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.TickFormula;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.AssetService.Controllers
{
    /// <summary>
    /// Manages tick formulas
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/tick-formulas")]
    public class TickFormulasController : ControllerBase, ITickFormulasApi
    {
        private readonly ITickFormulaService _tickFormulaService;
        private readonly IConvertService _convertService;

        public TickFormulasController(ITickFormulaService tickFormulaService, IConvertService convertService)
        {
            _tickFormulaService = tickFormulaService;
            _convertService = convertService;
        }

        /// <summary>
        /// Get tick formula by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetTickFormulaByIdResponse), (int) HttpStatusCode.OK)]
        public async Task<GetTickFormulaByIdResponse> GetByIdAsync([Required] string id)
        {
            var result = await _tickFormulaService.GetByIdAsync(id);

            var response = new GetTickFormulaByIdResponse();

            if (result == null)
            {
                response.Error = TickFormulaErrorCodesContract.TickFormulaDoesNotExist;
                return response;
            }

            response.TickFormula = _convertService.Convert<ITickFormula, TickFormulaContract>(result);

            return response;
        }

        /// <summary>
        /// Get all tick formulas
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(GetAllTickFormulasResponse), (int) HttpStatusCode.OK)]
        public async Task<GetAllTickFormulasResponse> GetAllAsync()
        {
            var result = await _tickFormulaService.GetAllAsync();

            return new GetAllTickFormulasResponse
            {
                TickFormulas = 
                    result.Select(x => _convertService.Convert<ITickFormula, TickFormulaContract>(x)).ToList()
            };
        }

        /// <summary>
        /// Adds new tick formula
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ErrorCodeResponse<TickFormulaErrorCodesContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<TickFormulaErrorCodesContract>> AddAsync(
            [FromBody] AddTickFormulaRequest request)
        {
            var model = _convertService.Convert<AddTickFormulaRequest, TickFormula>(request);

            var result = await _tickFormulaService.AddAsync(model, request.Username);

            var response = new ErrorCodeResponse<TickFormulaErrorCodesContract>();

            if (result.IsFailed)
            {
                response.ErrorCode =
                    _convertService.Convert<TickFormulaErrorCodes, TickFormulaErrorCodesContract>(result.Error
                        .Value);
            }

            return response;
        }

        /// <summary>
        /// Updates existing tick formula
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<TickFormulaErrorCodesContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<TickFormulaErrorCodesContract>> UpdateAsync(
            [FromBody] UpdateTickFormulaRequest request, [Required] string id)
        {
            var model = _convertService.Convert<UpdateTickFormulaRequest, TickFormula>(request);
            model.Id = id;

            var result = await _tickFormulaService.UpdateAsync(model, request.Username);

            var response = new ErrorCodeResponse<TickFormulaErrorCodesContract>();

            if (result.IsFailed)
            {
                response.ErrorCode =
                    _convertService.Convert<TickFormulaErrorCodes, TickFormulaErrorCodesContract>(result.Error
                        .Value);
            }

            return response;
        }

        /// <summary>
        /// Delete tick formula
        /// </summary>
        /// <param name="id"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<TickFormulaErrorCodesContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<TickFormulaErrorCodesContract>> DeleteAsync(
            [Required] string id, [Required] string username)
        {
            var result = await _tickFormulaService.DeleteAsync(id, username);

            var response = new ErrorCodeResponse<TickFormulaErrorCodesContract>();

            if (result.IsFailed)
            {
                response.ErrorCode =
                    _convertService.Convert<TickFormulaErrorCodes, TickFormulaErrorCodesContract>(result.Error
                        .Value);
            }

            return response;
        }
    }
}