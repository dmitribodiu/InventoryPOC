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
                Resolve.WhenEqualToHandlerMessageType(new PortfolioProjection()),
                new TransactionalSqlCommandExecutor(
                    SqlClientFactory.Instance,
                    @"Data Source=localhost;Initial Catalog=ProjacUsage;Integrated Security=SSPI;",
                    IsolationLevel.ReadCommitted));

            projector.Project(new List<object> { new DeleteData() });
            
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

                var subscription = connection.SubscribeToStreamFrom("portfolio-84cd61d6ed424276914da20de7a8c90f", StreamPosition.Start, CatchUpSubscriptionSettings.Default, (_, @event) =>
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
