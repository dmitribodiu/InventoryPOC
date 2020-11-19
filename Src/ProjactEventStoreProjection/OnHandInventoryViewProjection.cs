using Events;
using Events.Inventory;
using Projac.Sql;
using Projac.SqlClient;

namespace ProjactEventStoreProjection
{
    public class OnHandInventoryViewProjection : SqlProjection
    {
        private static readonly SqlClientSyntax Sql = new SqlClientSyntax();

        public OnHandInventoryViewProjection()
        {
            //When<DebitApplied>(@event =>
            //    Sql.NonQueryStatement(
            //        "INSERT INTO [Portfolio] ([Id], [Name], [PhotoCount]) VALUES (@P1, @P2, 0)",
            //        new { P1 = Sql.UniqueIdentifier(@event.Id), P2 = Sql.NVarChar(@event.Name, 40) }
            //    ));
            //When<CreditApplied>(@event =>
            //    Sql.NonQueryStatement(
            //        "DELETE FROM [Portfolio] WHERE [Id] = @P1",
            //        new { P1 = Sql.UniqueIdentifier(@event.Id) }
            //    ));
            //When<DeliveryScheduled>(@event =>
            //    Sql.NonQueryStatement(
            //        "UPDATE [Portfolio] SET [Name] = @P2 WHERE [Id] = @P1",
            //        new { P1 = Sql.UniqueIdentifier(@event.Id), P2 = Sql.NVarChar(@event.Name, 40) }
            //    ));

            //When<GeneralLedgerEntryPosted>(@event =>
            //    Sql.NonQueryStatement(
            //        "UPDATE [Portfolio] SET [Name] = @P2 WHERE [Id] = @P1",
            //        new { P1 = Sql.UniqueIdentifier(@event.Id), P2 = Sql.NVarChar(@event.Name, 40) }
            //    ));

            When<CreateSchema>(_ =>
                Sql.NonQueryStatement(
                    @"IF NOT EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='OnHandInventoryView' AND XTYPE='U')
                        BEGIN
                            CREATE TABLE [OnHandInventoryView] (
                                [Id] INT NOT NULL CONSTRAINT PK_OnHandInventory PRIMARY KEY AUTO_INCREMENT, 
                                [Location] NVARCHAR(MAX) NOT NULL,
                                [Amount] INT NOT NULL,
                                [SkuId] uniqueidentifier,
                                [IsReserved] BIT NOT NULL)
                        END"));
            When<DropSchema>(_ =>
                Sql.NonQueryStatement(
                    @"IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='OnHandInventoryView' AND XTYPE='U')
                        DROP TABLE [OnHandInventoryView]"));
            When<DeleteData>(_ =>
                Sql.NonQueryStatement(
                    @"IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='OnHandInventoryView' AND XTYPE='U')
                        DELETE FROM [OnHandInventoryView]"));
        }
    }
}
