using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ConsoleTables;
using Events;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Projac;

namespace OnHandInventoryInMemoryProjection
{
    class Program
    {
        static void Main(string[] args)
        {
            var credentials = new UserCredentials("admin", "changeit");

            var connectionSettings = ConnectionSettings.Create()
                .KeepReconnecting()
                .KeepRetrying()
                .FailOnNoServerResponse()
                .WithConnectionTimeoutOf(TimeSpan.FromSeconds(20))
                .SetOperationTimeoutTo(TimeSpan.FromSeconds(10))
                .SetHeartbeatInterval(TimeSpan.FromSeconds(10))
                .SetHeartbeatTimeout(TimeSpan.FromSeconds(20));

            MemoryCache cache = new MemoryCache(new MemoryCacheOptions());

            using (var connection = EventStoreConnection.Create("ConnectTo=tcp://admin:changeit@127.0.0.1:1113; HeartBeatTimeout=5000", connectionSettings))
            {
                connection.ConnectAsync().GetAwaiter().GetResult();

                var subscription = connection.SubscribeToStreamFrom("$ce-ledgerEntry", StreamPosition.Start, CatchUpSubscriptionSettings.Default, (_, @event) =>
                {
                    new Projector<MemoryCache>(
                            Resolve.WhenEqualToHandlerMessageType(InMemoryInventoryOverviewProjection.Projection.Handlers)).
                        ProjectAsync(cache, JsonConvert.DeserializeObject(
                            Encoding.UTF8.GetString(@event.Event.Data),
                            Assembly.GetAssembly(typeof(PortfolioRenamed)).GetType(@event.Event.EventType)));

                    var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
                    var collection = field.GetValue(cache) as ICollection;
                    if (collection != null)
                    {
                        var table = new ConsoleTable("SkuId", "Amount", "LocationId", "ReservationId, Batch");
                        foreach (var item in collection)
                        {
                            var methodInfo = item.GetType().GetProperty("Key");
                            var val = methodInfo.GetValue(item);
                            var value = cache.Get<StockLine>(val);
                            table.AddRow(value.SkuId, value.Amount, value.LocationId, value.ReservationId, value.Batch);
                        }
                        //Console.Clear();
                        Console.WriteLine($"EVENT TYPE: {@event.Event.EventType}");
                        table.Write();
                    }

                }, userCredentials: credentials);

                Console.ReadKey();
            }
        }
    }

    public class StockLine
    {
        public Guid SkuId { get; set; }
        public int Amount { get; set; }
        public Guid LocationId { get; set; }
        public Guid? ReservationId { get; set; }
        public string AccountId { get; set; }
        public string Batch { get; set; }
    }
}
