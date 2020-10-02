using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.AssetService.Contracts.Enums;
using MarginTrading.AssetService.Contracts.ProductCategories;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Handlers;
using MarginTrading.AssetService.Core.Services;

namespace MarginTrading.AssetService.Workflow.ProductCategories
{
    public class ProductCategoryChangedProjection
    {
        private readonly IReferentialDataChangedHandler _referentialDataChangedHandler;
        private readonly IConvertService _convertService;

        public ProductCategoryChangedProjection(IReferentialDataChangedHandler referentialDataChangedHandler, IConvertService convertService)
        {
            _referentialDataChangedHandler = referentialDataChangedHandler;
            _convertService = convertService;
        }

        [UsedImplicitly]
        public async Task Handle(ProductCategoryChangedEvent e)
        {
            switch (e.ChangeType)
            {
                case ChangeType.Creation:
                    break;
                case ChangeType.Edition:
                    await _referentialDataChangedHandler.HandleProductCategoryUpdated(
                        _convertService.Convert<ProductCategoryContract, ProductCategory>(e.NewProductCategory));
                    break;
                case ChangeType.Deletion:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}