update dbo.Products
set IsFrozen = old.IsFrozen,
    FreezeInfo = old.FreezeInfo,
    IsSuspended = old.IsSuspended,
    IsDiscontinued = old.IsDiscontinued

from dbo.Products
         inner join
     dbo.AssetPairs old
     on old.Id = ProductId