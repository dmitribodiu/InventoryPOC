using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Events;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;

namespace WriteSideTestClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var credentials = new UserCredentials("admin", "changeit");

                var portfolioId = Guid.Parse("474f7597-3c81-4993-a331-ec3c04f96e44");
                var events = new object[]
                {
                    new PortfolioAdded {Id = portfolioId, Name = "My Portfolio"},
                    new PortfolioRenamed {Id = portfolioId, Name = "Your Portfolio"},
                    new PortfolioRemoved {Id = portfolioId}
                };
                var stream = string.Format("portfolio-{0}", portfolioId.ToString("N"));

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
                    await connection.ConnectAsync();

                    await connection.AppendToStreamAsync(
                        stream,
                        ExpectedVersion.Any,
                        events.Select(@event => new EventData(
                            Guid.NewGuid(),
                            @event.GetType().FullName,
                            true,
                            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)),
                            new byte[0])).ToArray());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        

            
            Console.WriteLine("Hello World!");
        }
    }
}
