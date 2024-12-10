BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[Products]') AND [c].[name] = N'ActualDiscontinuedDate');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [dbo].[Products] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [dbo].[Products] DROP COLUMN [ActualDiscontinuedDate];
GO

DELETE FROM [dbo].[__EFMigrationsHistory]
WHERE [MigrationId] = N'20240906125527_Product_ActualDiscontinuedDate';
GO

COMMIT;
GO

