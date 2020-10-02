using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.AssetService.Contracts.Enums;
using MarginTrading.AssetService.Contracts.Products;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Handlers;
using MarginTrading.AssetService.Core.Services;

namespace MarginTrading.AssetService.Workflow.Products
{
    public class ProductChangedProjection
    {
        private readonly IReferentialDataChangedHandler _referentialDataChangedHandler;
        private readonly IConvertService _convertService;

        public ProductChangedProjection(IReferentialDataChangedHandler referentialDataChangedHandler, IConvertService convertService)
        {
            _referentialDataChangedHandler = referentialDataChangedHandler;
            _convertService = convertService;
        }

        [UsedImplicitly]
        public async Task Handle(ProductChangedEvent e)
        {
            switch (e.ChangeType)
            {
                case ChangeType.Creation:
                case ChangeType.Edition:
                    await _referentialDataChangedHandler.HandleProductUpserted(_convertService.Convert<ProductContract, Product>(e.NewValue));
                    break;
                case ChangeType.Deletion:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}