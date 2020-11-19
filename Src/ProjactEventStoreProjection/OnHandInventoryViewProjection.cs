using System.Collections.Generic;
using System.Linq;
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
            When<DebitApplied>(@event => DebitAppliedHandler(@event));
            When<CreditApplied>(@event => CreditAppliedHandler(@event));
            When<DeliveryScheduled>(@event => DeliveryScheduledHandler(@event));
            When<GeneralLedgerEntryPosted>(@event => GeneralLedgerEntryPostedHandler(@event));

            When<CreateSchema>(_ =>
                Sql.NonQueryStatement(
                    @"IF NOT EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='OnHandInventoryView' AND XTYPE='U')
                        BEGIN
                            CREATE TABLE [OnHandInventoryView] (
                                [Id] INT IDENTITY(1,1) PRIMARY KEY , 
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

        private IEnumerable<SqlNonQueryCommand> GeneralLedgerEntryPostedHandler(GeneralLedgerEntryPosted @event)
        {
            return Sql.NonQueryStatementIf(false, "");
        }

        private IEnumerable<SqlNonQueryCommand> DeliveryScheduledHandler(DeliveryScheduled @event)
        {
            return Sql.NonQueryStatementIf(false, "");
        }

        private IEnumerable<SqlNonQueryCommand> CreditAppliedHandler(CreditApplied @event)
        {
            return Sql.NonQueryStatementIf(false, "");
        }

        private IEnumerable<SqlNonQueryCommand> DebitAppliedHandler(DebitApplied @event)
        {
            var lastAccount = @event.Account.Split(":").Last();
            var lastAccountPrefix = lastAccount.Split("|").First();
            var lastAccountId = lastAccount.Split("|").Last();

            return Sql.NonQueryStatementIf(lastAccountPrefix == "WL",
                @"declare @Amount int = (select top 1 Amount FROM [OnHandInventoryView] where skuId = @SkuId and IsReserved = 0 and location = @Location)
                            IF(@Amount IS NULL) 
	                            INSERT INTO [OnHandInventoryView] ([Location],[Amount],[SkuId],[IsReserved]) VALUES (@Location, @AmountToAppend, @SkuId, 0)
                            ELSE
	                            UPDATE [OnHandInventoryView] SET [Amount] = @Amount + @AmountToAppend WHERE SkuId = @SkuId and IsReserved = 0",
                new
                {
                    SkuId = Sql.UniqueIdentifier(@event.SkuId), AmountToAppend = Sql.Int(@event.Amount),
                    Location = Sql.VarChar(lastAccountId, 50)
                });
        }
    }
}
