-- Copy the output of the 1st script into this script
-- Execute the modified script 2 on the db with dbo.Products

BEGIN TRANSACTION;

-- 1. Create a temp table
CREATE TABLE #Und (
    MdsCode NVARCHAR(100),
    ExpiryDate DATE
);

-- 2. Fill the temp table with values from MDM Underlyings

-- !!! REPLACE THIS LINE WITH INSERT STATEMENTS FROM SCRIPT 1

-- 3. Execute the main migration script
UPDATE Product
SET Product.[ActualDiscontinuedDate] = #Und.ExpiryDate
FROM [dbo].[Products] AS Product
INNER JOIN #Und
ON Product.UnderlyingMdsCode = #Und.MdsCode
WHERE Product.IsDiscontinued = 1;

-- 4. Clean up the temp table
DROP TABLE #Und;

COMMIT;