using System.Collections.Generic;
using System.Linq;
using MarginTrading.AssetService.Core.Domain;
using Xunit;

namespace MarginTrading.AssetService.Tests.UnitTests
{
    public class ProductCategoryNameTests
    {
        [Fact]
        public void SimpleCategoryName()
        {
            var originalName = "Stocks";
            var normalizedName = "stocks";

            var productCategoryName = new ProductCategoryName(originalName, normalizedName);

            Assert.Single(productCategoryName.Nodes);

            var nodeName = productCategoryName.GetOriginalNodeName("stocks"); 
            Assert.Equal(originalName, nodeName);

            var node = productCategoryName.Nodes.First();
            Assert.Equal("stocks", node.Id);
            Assert.True(node.IsLeaf);
            Assert.Null(node.ParentId);
        }

        [Fact]
        public void CategoryNameTree()
        {
            var originalName = "Stocks/Germany/Dax 30";
            var normalizedName = "stocks.germany.dax_30";

            var expectedValues = new Dictionary<string, (string OriginalName, string ParentId, bool IsLeaf)>
            {
                {"stocks", ("Stocks", null, false)},
                {"stocks.germany", ("Germany", "stocks", false)},
                {"stocks.germany.dax_30", ("Dax 30", "stocks.germany", true)}
            };

            var productCategoryName = new ProductCategoryName(originalName, normalizedName);

            Assert.Equal(3, productCategoryName.Nodes.Count);

            foreach (var (id, expected) in expectedValues)
            {
                var node = productCategoryName.Nodes.FirstOrDefault(n => n.Id == id);
                Assert.NotNull(node);

                var originalNodeName = productCategoryName.GetOriginalNodeName(id);
                Assert.Equal(expected.OriginalName, originalNodeName);
                
                Assert.Equal(expected.ParentId, node.ParentId);
                Assert.Equal(expected.IsLeaf, node.IsLeaf);
            }
        }

        [Fact]
        public void NormalizedCategoryNameTree()
        {
            var originalName = "stocks.germany.dax_30";
            var normalizedName = "stocks.germany.dax_30";

            var expectedValues = new Dictionary<string, (string OriginalName, string ParentId, bool IsLeaf)>
            {
                {"stocks", ("Stocks", null, false)},
                {"stocks.germany", ("Germany", "stocks", false)},
                {"stocks.germany.dax_30", ("Dax 30", "stocks.germany", true)}
            };

            var productCategoryName = new ProductCategoryName(originalName, normalizedName);

            Assert.Equal(3, productCategoryName.Nodes.Count);

            foreach (var (id, expected) in expectedValues)
            {
                var node = productCategoryName.Nodes.FirstOrDefault(n => n.Id == id);
                Assert.NotNull(node);

                var originalNodeName = productCategoryName.GetOriginalNodeName(id);
                Assert.Null(originalNodeName);
                
                Assert.Equal(expected.ParentId , node.ParentId);
                Assert.Equal(expected.IsLeaf,  node.IsLeaf);
            }
        }
    }
}