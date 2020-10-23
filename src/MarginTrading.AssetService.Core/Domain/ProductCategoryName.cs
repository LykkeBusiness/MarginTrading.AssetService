using System.Collections.Generic;

namespace MarginTrading.AssetService.Core.Domain
{
    // todo: tests
    public class ProductCategoryName
    {
        private readonly string _originalName;
        private readonly Dictionary<string, string> _originalNodeNames = new Dictionary<string, string>();
        public string NormalizedName { get; }

        public List<ProductCategory> Nodes { get; } = new List<ProductCategory>();

        public ProductCategoryName(string originalName, string normalizedName)
        {
            _originalName = originalName;
            NormalizedName = normalizedName;

            Init();
        }

        public string GetOriginalNodeName(string normalizedCategoryId)
        {
            return _originalNodeNames.TryGetValue(normalizedCategoryId, out var result) ? result : null;
        }

        private void Init()
        {
            var originalNames = _originalName.Split('/');
            var normalizedNames = NormalizedName.Split('.');

            string currentId = null;
            string parentId = null;

            for (var i = 0; i < normalizedNames.Length; i++)
            {
                currentId = i == 0 ? normalizedNames[0] : string.Join('.', currentId, normalizedNames[i]);
                var productCategory = new ProductCategory()
                {
                    Id = currentId,
                    LocalizationToken = $"categoryName.{currentId}",
                    ParentId = parentId,
                    IsLeaf = i == normalizedNames.Length - 1,
                };

                Nodes.Add(productCategory);
                if (originalNames.Length == normalizedNames.Length)
                {
                    _originalNodeNames.Add(currentId, originalNames[i]);
                }

                parentId = currentId;
            }
        }
    }
}