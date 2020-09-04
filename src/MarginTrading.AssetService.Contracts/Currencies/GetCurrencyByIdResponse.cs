using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.ErrorCodes;

namespace MarginTrading.AssetService.Contracts.Currencies
{
    public class GetCurrencyByIdResponse : ErrorCodeResponse<CurrenciesErrorCodesContract>
    {
        public CurrencyContract Currency { get; set; }
    }
}