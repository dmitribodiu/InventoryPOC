using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Events;
using Events.Inventory;
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
                // DeliveryScheduled, GoodsArrivedOnSite, GoodsUnloaded, GoodsReserved, GoodsLoaded, GoodsLeftSite, GoodsShifted.
                // accounts: 


                //PostGeneralLedgerEntry:
                //{ id = guid,
                //  PostDAte: Now,
                //  CreatedOn: Yesterday,
                //  BusinessTransaction: { ReferenceNumber: 'deliveryScheduled-DeliveryId', InboundDeliveryId: guid, SkuId: A, SkuAmount: 20  } }

                // Transaction:
                var entryId = Guid.NewGuid();
                var entryPostDate = DateTimeOffset.UtcNow;
                var customerId = Guid.NewGuid();
                var inboundDeliveryId = Guid.NewGuid();
                var skuId = Guid.NewGuid();

                var deliveryScheduled = new PostGeneralLedgerEntry
                {
                    CreatedOn = DateTimeOffset.UtcNow,
                    PostDate = entryPostDate,
                    GeneralLedgerEntryId = entryId,
                    BusinessTransaction = new DeliveryScheduledTransaction
                    {
                        ReferenceNumber = 1,
                        CustomerId = customerId,
                        InboundDeliveryId = inboundDeliveryId,
                        SkuId = skuId,
                        Amount = 20
                    }
                };

                // Should be translated into:
                var deliveryScheduledEvents = new object[]
                {
                    new CreditApplied { Account = $"C-{customerId}", GeneralLedgerEntryId = entryId, Amount = 20, SkuId = skuId },
                    new DebitApplied { Account = $"C-{customerId}:ID-{inboundDeliveryId}", GeneralLedgerEntryId = entryId, Amount = 20, SkuId = skuId },
                    deliveryScheduled.BusinessTransaction.GetAdditionalChanges(),
                    new GeneralLedgerEntryPosted { GeneralLedgerEntryId = entryId, PostDate = entryPostDate }
                };

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
                        $"ledgerEntry-{entryId}",
                        ExpectedVersion.Any,
                        deliveryScheduledEvents.Select(@event => new EventData(
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
