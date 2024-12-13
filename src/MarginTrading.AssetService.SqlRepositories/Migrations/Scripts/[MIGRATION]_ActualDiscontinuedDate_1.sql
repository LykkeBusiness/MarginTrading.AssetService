-- Execute the script on the db with mdm.Underlyings. It will generate INSERT statements with MdsCode and ExpiryDate. Use it in the script 2. 

SELECT 
    'INSERT INTO #Und (MdsCode, ExpiryDate) VALUES (''' + 
    MdsCode + ''', ''' + 
    CONVERT(VARCHAR(10), ExpiryDate, 120) + ''');'
FROM [mdm].[Underlyings]
where ExpiryDate is not null