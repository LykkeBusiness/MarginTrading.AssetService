using System.Collections.Generic;
using System.Linq;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Helpers
{
    public class CategoryLeafHelper
    {
        public List<ProductCategory> LeafNodes { get; set; }

        public CategoryLeafHelper(List<ProductCategory> nodes)
        {
            LeafNodes = nodes
                .Where(n => n.IsLeaf 
                            && !nodes.Exists(n1 => n1.Id == n.Id && !n1.IsLeaf))
                .ToList();
        }

        public bool IsLeaf(string node)
        {
            return LeafNodes.Exists(n => n.Id == node);
        }
    }
}