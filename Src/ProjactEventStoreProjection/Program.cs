using System;
using System.Collections.Generic;
using System.Data;
using Projac.Sql;
using Projac.Sql.Executors;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Events;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;

namespace ProjactEventStoreProjection
{
    class Program
    {
        static void Main(string[] args)
        {
            var projector = new SqlProjector(
                Resolve.WhenEqualToHandlerMessageType(new OnHandInventoryViewProjection()),
                new TransactionalSqlCommandExecutor(
                    SqlClientFactory.Instance,
                    @"Data Source=localhost;Initial Catalog=InventoryPOC;Integrated Security=SSPI;",
                    IsolationLevel.ReadCommitted));

            projector.Project(new List<object> { new DropSchema(), new CreateSchema() });
            
            var credentials = new UserCredentials("admin", "changeit");

            var connectionSettings = ConnectionSettings.Create()
                .KeepReconnecting()
                .KeepRetrying()
                .FailOnNoServerResponse()
                .WithConnectionTimeoutOf(TimeSpan.FromSeconds(20))
                .SetOperationTimeoutTo(TimeSpan.FromSeconds(10))
                .SetHeartbeatInterval(TimeSpan.FromSeconds(10))
                .SetHeartbeatTimeout(TimeSpan.FromSeconds(20));

            using (var connection = EventStoreConnection.Create("ConnectTo=tcp://admin:changeit@127.0.0.1:1113; HeartBeatTimeout=5000", connectionSettings))
            {
                connection.ConnectAsync().GetAwaiter().GetResult();

                var subscription = connection.SubscribeToStreamFrom("$ce-ledgerEntry", StreamPosition.Start, CatchUpSubscriptionSettings.Default, (_, @event) =>
                {

                    projector.Project(
                        JsonConvert.DeserializeObject(
                            Encoding.UTF8.GetString(@event.Event.Data),
                            Assembly.GetAssembly(typeof(PortfolioRenamed)).GetType(@event.Event.EventType)));
                }, userCredentials: credentials);

                var a = subscription.LastProcessedEventNumber;
                Console.ReadKey();
            }

            
        }
    }
}
