using Events;
using Projac.Sql;
using Projac.SqlClient;

namespace ProjactEventStoreProjection
{
    public class PortfolioProjection : SqlProjection
    {
        private static readonly SqlClientSyntax Sql = new SqlClientSyntax();

        public PortfolioProjection()
        {
            When<PortfolioAdded>(@event =>
                Sql.NonQueryStatement(
                    "INSERT INTO [Portfolio] ([Id], [Name], [PhotoCount]) VALUES (@P1, @P2, 0)",
                    new { P1 = Sql.UniqueIdentifier(@event.Id), P2 = Sql.NVarChar(@event.Name, 40) }
                ));
            When<PortfolioRemoved>(@event =>
                Sql.NonQueryStatement(
                    "DELETE FROM [Portfolio] WHERE [Id] = @P1",
                    new { P1 = Sql.UniqueIdentifier(@event.Id) }
                ));
            When<PortfolioRenamed>(@event =>
                Sql.NonQueryStatement(
                    "UPDATE [Portfolio] SET [Name] = @P2 WHERE [Id] = @P1",
                    new { P1 = Sql.UniqueIdentifier(@event.Id), P2 = Sql.NVarChar(@event.Name, 40) }
                ));

            When<CreateSchema>(_ =>
                Sql.NonQueryStatement(
                    @"IF NOT EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='Portfolio' AND XTYPE='U')
                        BEGIN
                            CREATE TABLE [Portfolio] (
                                [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Portfolio PRIMARY KEY, 
                                [Name] NVARCHAR(MAX) NOT NULL,
                                [PhotoCount] INT NOT NULL)
                        END"));
            When<DropSchema>(_ =>
                Sql.NonQueryStatement(
                    @"IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='Portfolio' AND XTYPE='U')
                        DROP TABLE [Portfolio]"));
            When<DeleteData>(_ =>
                Sql.NonQueryStatement(
                    @"IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='Portfolio' AND XTYPE='U')
                        DELETE FROM [Portfolio]"));
        }
    }
}