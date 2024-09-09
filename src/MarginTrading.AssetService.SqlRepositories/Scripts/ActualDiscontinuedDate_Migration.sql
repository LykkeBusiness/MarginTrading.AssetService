-- This is file for migrating ActualDiscontinuedDate in products table. Its not implemented in ef migration due to we need to access Mdm database which can be on different server

-- Execution

UPDATE Product
SET Product.[ActualDiscontinuedDate] = Underlying.ExpiryDate
    FROM [dbo].[Products] AS Product
    INNER JOIN [mdm].[Underlyings] as Underlying  -- keep in mind that mdm.Underlyings can be not accessible
    ON Product.UnderlyingMdsCode = Underlying.MdsCode


-- Rollback
UPDATE [dbo].[Products]
SET [ActualDiscontinuedDate] = NULL
