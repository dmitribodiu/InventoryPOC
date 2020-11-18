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
        static void Main(string[] args)
        {
            try
            {
                var credentials = new UserCredentials("admin", "changeit");

                var portfolioId = Guid.Parse("84cd61d6-ed42-4276-914d-a20de7a8c90f");
                var events = new object[]
                {
                    new PortfolioRemoved {Id = portfolioId}, //only because Start from 0 doesn't work on CatchUpSubscribtion.
                    new PortfolioAdded {Id = portfolioId, Name = "My Portfolio"},
                    new PortfolioRenamed {Id = portfolioId, Name = "Your Portfolio"},
                    //new PortfolioRemoved {Id = portfolioId}
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
                    connection.ConnectAsync().GetAwaiter().GetResult();

                    connection.AppendToStreamAsync(
                        stream,
                        ExpectedVersion.Any,
                        events.Select(@event => new EventData(
                            Guid.NewGuid(),
                            @event.GetType().FullName,
                            true,
                            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)),
                            new byte[0])).ToArray()).GetAwaiter().GetResult();
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
