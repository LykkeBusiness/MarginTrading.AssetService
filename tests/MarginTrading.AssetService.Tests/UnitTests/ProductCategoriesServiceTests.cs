using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Snow.Common.Correlation;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Services;
using MarginTrading.AssetService.Services.Validations.Products;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Moq;
using Xunit;

namespace MarginTrading.AssetService.Tests.UnitTests
{
    public class ProductCategoriesServiceTests
    {
        private readonly Mock<IProductCategoriesRepository> _productCategoriesRepositoryMock =
            new Mock<IProductCategoriesRepository>();

        private readonly Mock<IAuditService> _auditServiceMock = new Mock<IAuditService>();
        private readonly Mock<ICqrsMessageSender> _cqrsMessageSenderMock = new Mock<ICqrsMessageSender>();
        private readonly Mock<IConvertService> _convertServiceMock = new Mock<IConvertService>();
        private readonly Mock<CorrelationContextAccessor> _correlationContextAccessor = new Mock<CorrelationContextAccessor>();
        private readonly Mock<IIdentityGenerator> _identityGenerator = new Mock<IIdentityGenerator>();

        [Fact]
        public async Task OneProductInParentCategory()
        {
            var pairs = new List<ProductAndCategoryPair>()
            {
                new ProductAndCategoryPair()
                {
                    Category = "stocks",
                    ProductId = "product1",
                }
            };
            
            var service = GetService();

            var errorMessages = await service.Validate(pairs);
            
            Assert.Empty(errorMessages);
        }
        
        [Fact]
        public async Task OneProductInChildCategory()
        {
            var pairs = new List<ProductAndCategoryPair>()
            {
                new ProductAndCategoryPair()
                {
                    Category = "stocks/Germany",
                    ProductId = "product1",
                }
            };
            
            var service = GetService();

            var errorMessages = await service.Validate(pairs);
            
            Assert.Empty(errorMessages);
        }
        
        [Fact]
        public async Task OneProductInChildNormalizedCategory()
        {
            var pairs = new List<ProductAndCategoryPair>()
            {
                new ProductAndCategoryPair()
                {
                    Category = "stocks.germany.dax_30",
                    ProductId = "product1",
                }
            };
            
            var service = GetService();

            var errorMessages = await service.Validate(pairs);
            
            Assert.Empty(errorMessages);
        }
        
        [Fact]
        public async Task InvalidCategoryName()
        {
            var pairs = new List<ProductAndCategoryPair>()
            {
                new ProductAndCategoryPair()
                {
                    Category = " ",
                    ProductId = "product1",
                }
            };
            
            var service = GetService();

            var errorMessages = await service.Validate(pairs);
            
            Assert.Single(errorMessages);
            Assert.Equal("Category \" \" is not a valid category",
                errorMessages.First());
        }
        
        [Fact]
        public async Task ProductMustBeCreatedInLeafCategory()
        {
            var pairs = new List<ProductAndCategoryPair>()
            {
                new ProductAndCategoryPair()
                {
                    Category = "stocks/Germany",
                    ProductId = "product1",
                },
                new ProductAndCategoryPair()
                {
                    Category = "stocks",
                    ProductId = "product2",
                }
            };
            
            var service = GetService();

            var errorMessages = await service.Validate(pairs);
            
            Assert.Single(errorMessages);
            Assert.Equal("Product product2 cannot be attached to a category stocks because it is not a leaf category",
                errorMessages.First());
        }

        private IProductCategoriesService GetService()
        {
            return new ProductCategoriesService(_productCategoriesRepositoryMock.Object,
                new ProductCategoryStringService(new ProductCategoryStringValidation()),
                _auditServiceMock.Object,
                _cqrsMessageSenderMock.Object,
                _convertServiceMock.Object,
                _correlationContextAccessor.Object);
        }
    }
}