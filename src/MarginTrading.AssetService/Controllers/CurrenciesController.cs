using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Snow.Common;
using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.Currencies;
using MarginTrading.AssetService.Contracts.ErrorCodes;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.AssetService.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/currencies")]
    public class CurrenciesController : ControllerBase, ICurrenciesApi
    {
        private readonly ICurrenciesService _currenciesService;
        private readonly IConvertService _convertService;

        public CurrenciesController(ICurrenciesService currenciesService, IConvertService convertService)
        {
            _currenciesService = currenciesService;
            _convertService = convertService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ErrorCodeResponse<CurrenciesErrorCodesContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<CurrenciesErrorCodesContract>> AddAsync(
            [FromBody] AddCurrencyRequest request)
        {
            var currency = _convertService.Convert<AddCurrencyRequest, Currency>(request);
            var result = await _currenciesService.InsertAsync(currency, request.UserName);

            var response = new ErrorCodeResponse<CurrenciesErrorCodesContract>();

            if (result.IsFailed)
            {
                response.ErrorCode =
                    _convertService.Convert<CurrenciesErrorCodes, CurrenciesErrorCodesContract>(result.Error.Value);
            }

            return response;
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<CurrenciesErrorCodesContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<CurrenciesErrorCodesContract>> UpdateAsync(
            [FromRoute] [Required] string id, [FromBody] UpdateCurrencyRequest request)
        {
            var currency = _convertService.Convert<UpdateCurrencyRequest, Currency>(request);
            currency.Id = id;
            
            var result = await _currenciesService.UpdateAsync(currency, request.UserName);

            var response = new ErrorCodeResponse<CurrenciesErrorCodesContract>();

            if (result.IsFailed)
            {
                response.ErrorCode =
                    _convertService.Convert<CurrenciesErrorCodes, CurrenciesErrorCodesContract>(result.Error.Value);
            }

            return response;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<CurrenciesErrorCodesContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<CurrenciesErrorCodesContract>> DeleteAsync(
            [FromRoute] [Required] string id, [FromBody] DeleteCurrencyRequest request)
        {
            var result = await _currenciesService.DeleteAsync(id, request.UserName);

            var response = new ErrorCodeResponse<CurrenciesErrorCodesContract>();

            if (result.IsFailed)
            {
                response.ErrorCode =
                    _convertService.Convert<CurrenciesErrorCodes, CurrenciesErrorCodesContract>(result.Error.Value);
            }

            return response;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetCurrencyByIdResponse), (int) HttpStatusCode.OK)]
        public async Task<GetCurrencyByIdResponse> GetByIdAsync([FromRoute] [Required] string id)
        {
            var result = await _currenciesService.GetByIdAsync(id);

            var response = new GetCurrencyByIdResponse();

            if (result.IsSuccess)
            {
                response.Currency = _convertService.Convert<Currency, CurrencyContract>(result.Value);
            }

            if (result.IsFailed)
            {
                response.ErrorCode =
                    _convertService.Convert<CurrenciesErrorCodes, CurrenciesErrorCodesContract>(result.Error.Value);
            }

            return response;
        }

        [HttpGet]
        [ProducesResponseType(typeof(GetCurrenciesResponse), (int) HttpStatusCode.OK)]
        public async Task<GetCurrenciesResponse> GetAllAsync([FromQuery] int skip = 0, [FromQuery] int take = 0)
        {
            
            (skip, take) = PaginationUtils.ValidateSkipAndTake(skip, take);

            // if take == 0 return all rows
            var result = take == 0
                ? await _currenciesService.GetAllAsync()
                : await _currenciesService.GetByPageAsync(skip, take);

            var response = new GetCurrenciesResponse
            {
                Currencies = result.Value
                    .Select(value => _convertService.Convert<Currency, CurrencyContract>(value))
                    .ToList()
            };

            return response;
        }
    }
}