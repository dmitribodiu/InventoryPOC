using System;
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
                                [ReservationId] uniqueidentifier)
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
            var result = new List<SqlNonQueryCommand>();

            var lastAccount = @event.Account.Split(":").Last();
            var lastAccountPrefix = lastAccount.Split("|").First();
            var lastAccountId = lastAccount.Split("|").Last();

            if (lastAccountPrefix == "WL")
            {
                var command = Sql.NonQueryStatement(
                    @"declare @Amount int = (select top 1 Amount FROM [OnHandInventoryView] where skuId = @SkuId and ReservationId IS NULL and location = @Location)
                            IF(@Amount IS NULL) 
	                            INSERT INTO [OnHandInventoryView] ([Location],[Amount],[SkuId],[ReservationId]) VALUES (@Location, @AmountToAppend, @SkuId, NULL)
                            ELSE
	                            UPDATE [OnHandInventoryView] SET [Amount] = @Amount + @AmountToAppend WHERE SkuId = @SkuId and ReservationId IS NULL",
                    new
                    {
                        SkuId = Sql.UniqueIdentifier(@event.SkuId),
                        AmountToAppend = Sql.Int(@event.Amount),
                        Location = Sql.VarChar(lastAccountId, 50)
                    });

                result.Add(command);
            }
            else if(lastAccountPrefix == "R")
            {
                var penUltimateAccount = @event.Account.Split(":").Reverse().Skip(1).First();
                var penultimateAccountId = penUltimateAccount.Split("|").Last();

                var command = Sql.NonQueryStatement(
                    @"declare @AvailableAmount int = (select top 1 Amount FROM [OnHandInventoryView] where skuId = @SkuId and ReservationId IS NULL and Location = @Location)
                           declare @Amount int = (select top 1 Amount FROM [OnHandInventoryView] where skuId = @SkuId and ReservationId = @ReservationId and Location = @Location)

                            IF(@Amount IS NULL)
							BEGIN 
	                            INSERT INTO [OnHandInventoryView] ([Location],[Amount],[SkuId],[ReservationId]) VALUES (@Location, @ReservedAmount, @SkuId, @ReservationId)
                                UPDATE [OnHandInventoryView] SET [Amount] = @AvailableAmount - @ReservedAmount WHERE SkuId = @SkuId and ReservationId IS NULL and Location = @Location
							END
                            ELSE
							BEGIN
	                            UPDATE [OnHandInventoryView] SET [Amount] = @Amount + @ReservedAmount WHERE SkuId = @SkuId and ReservationId = @ReservationId
                                UPDATE [OnHandInventoryView] SET [Amount] = @AvailableAmount - @ReservedAmount WHERE SkuId = @SkuId and ReservationId IS NULL and Location = @Location
								END",
                    new
                    {
                        SkuId = Sql.UniqueIdentifier(@event.SkuId),
                        ReservedAmount = Sql.Int(@event.Amount),
                        Location = Sql.VarChar(penultimateAccountId, 50),
                        ReservationId = Sql.UniqueIdentifier(Guid.Parse(lastAccountId))
                    });

                result.Add(command);
            }

            return result;
        }
    }
}
