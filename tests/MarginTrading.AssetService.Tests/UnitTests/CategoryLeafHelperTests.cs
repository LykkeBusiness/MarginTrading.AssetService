using System.Collections.Generic;
using System.Linq;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Helpers;
using Xunit;

namespace MarginTrading.AssetService.Tests.UnitTests
{
    public class CategoryLeafHelperTests
    {
        [Fact]
        public void ParentTest()
        {
            var category = "stocks";
       
            var categoryName = new ProductCategoryName(category, category);

            var helper = new CategoryLeafHelper(categoryName.Nodes);
            
            Assert.Single(helper.LeafNodes);
            Assert.Equal("stocks", helper.LeafNodes.First().Id);
        }
        
        [Fact]
        public void SubcategoryTest()
        {
            var category1 = "stocks";
            var category2 = "stocks.germany";
            
            var categoryName1 = new ProductCategoryName(category1, category1);
            var categoryName2 = new ProductCategoryName(category2, category2);

            var allNodes = new List<ProductCategory>();
            allNodes.AddRange(categoryName1.Nodes);
            allNodes.AddRange(categoryName2.Nodes);
            
            var helper = new CategoryLeafHelper(allNodes);
            
            Assert.Single(helper.LeafNodes);
            Assert.Equal("stocks.germany", helper.LeafNodes.First().Id);
        }
        
        [Fact]
        public void TwoParentsTest()
        {
            var category1 = "stocks";
            var category2 = "futures";
            
            var categoryName1 = new ProductCategoryName(category1, category1);
            var categoryName2 = new ProductCategoryName(category2, category2);
            

            var allNodes = new List<ProductCategory>();
            allNodes.AddRange(categoryName1.Nodes);
            allNodes.AddRange(categoryName2.Nodes);

            var helper = new CategoryLeafHelper(allNodes);
            
            Assert.Equal(2, helper.LeafNodes.Count);
        }
        
        [Fact]
        public void TwoParentsOneSubcategoryTest()
        {
            var category1 = "stocks";
            var category2 = "stocks.germany";
            var category3 = "futures";
            
            var categoryName1 = new ProductCategoryName(category1, category1);
            var categoryName2 = new ProductCategoryName(category2, category2);
            var categoryName3 = new ProductCategoryName(category3, category3);

            var allNodes = new List<ProductCategory>();
            allNodes.AddRange(categoryName1.Nodes);
            allNodes.AddRange(categoryName2.Nodes);
            allNodes.AddRange(categoryName3.Nodes);
            
            var helper = new CategoryLeafHelper(allNodes);
            
            Assert.Equal(2, helper.LeafNodes.Count);
            
            Assert.Contains(helper.LeafNodes, node => node.Id == "stocks.germany");
            Assert.Contains(helper.LeafNodes, node => node.Id == "futures");
        }
    }
}