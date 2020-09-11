using System.Collections.Generic;
using System.Linq;

namespace MarginTrading.AssetService.Core.Extensions
{
    public static class ListExtensions
    {
        public static bool IsAscendingSorted(this IList<decimal> list)
        {
            for (var i = 1; i < list.Count; i++)
            {
                if (list[i - 1] > list[i])
                    return false;
            }
            return true;
        }

        public static bool IsAscendingSortedWithNoDuplicates(this IList<decimal> list)
        {
            for (var i = 1; i < list.Count; i++)
            {
                if (list[i - 1] >= list[i])
                    return false;
            }
            return true;
        }
    }
}