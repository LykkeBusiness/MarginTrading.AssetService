using MarginTrading.AssetService.StorageInterfaces.Repositories;

namespace MarginTrading.AssetService.Services
{
    public class AssetsService
    {
        private readonly ICurrenciesRepository _currenciesRepository;
        private readonly IProductsRepository _productsRepository;

        public AssetsService(ICurrenciesRepository currenciesRepository, IProductsRepository productsRepository)
        {
            _currenciesRepository = currenciesRepository;
            _productsRepository = productsRepository;
        }
    }
}