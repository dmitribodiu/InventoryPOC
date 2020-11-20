using System;
using System.Linq;
using System.Text;
using Events.Inventory;
using EventStore.ClientAPI;
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
                var deliveryScheduledEntryId = Guid.NewGuid();
                var deliveryScheduledEntryPostDate = DateTimeOffset.UtcNow;
                var customerId = Guid.NewGuid();
                var inboundDeliveryId = Guid.NewGuid();
                var skuId = Guid.NewGuid();

                var deliveryScheduled = new PostGeneralLedgerEntry
                {
                    CreatedOn = DateTimeOffset.UtcNow,
                    PostDate = deliveryScheduledEntryPostDate,
                    GeneralLedgerEntryId = deliveryScheduledEntryId,
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
                    new CreditApplied { Account = $"C|{customerId}", GeneralLedgerEntryId = deliveryScheduledEntryId, Amount = 20, SkuId = skuId },
                    new DebitApplied { Account = $"C|{customerId}:ID|{inboundDeliveryId}", GeneralLedgerEntryId = deliveryScheduledEntryId, Amount = 20, SkuId = skuId },
                    deliveryScheduled.BusinessTransaction.GetAdditionalChanges(),
                    new GeneralLedgerEntryPosted { GeneralLedgerEntryId = deliveryScheduledEntryId, PostDate = deliveryScheduledEntryPostDate }
                };

                // Goods unloaded:

                var goodsUnloadedEntryId = Guid.NewGuid();
                var goodsUnloadedEntryPostDate = DateTimeOffset.UtcNow;
                Guid locationId = Guid.NewGuid();

                var goodsUnloaded = new PostGeneralLedgerEntry
                {
                    CreatedOn = DateTimeOffset.UtcNow,
                    PostDate = goodsUnloadedEntryPostDate,
                    GeneralLedgerEntryId = goodsUnloadedEntryId,
                    BusinessTransaction = new GoodsUnloadedTransaction
                    {
                        ReferenceNumber = 1,
                        CustomerId = customerId,
                        InboundDeliveryId = inboundDeliveryId,
                        LocationId = locationId,
                        WorkOrderId = Guid.NewGuid(),
                        SkuId = skuId,
                        Amount = 20
                    }
                };

                // Should be translated into:
                var goodsUnloadedEvents = new object[]
                {
                    new CreditApplied { Account = $"C|{customerId}:ID|{inboundDeliveryId}", GeneralLedgerEntryId = goodsUnloadedEntryId, Amount = 20, SkuId = skuId },
                    new DebitApplied { Account = $"C|{customerId}:WL|{locationId}", GeneralLedgerEntryId = goodsUnloadedEntryId, Amount = 20, SkuId = skuId },
                    goodsUnloaded.BusinessTransaction.GetAdditionalChanges(),
                    new GeneralLedgerEntryPosted { GeneralLedgerEntryId = goodsUnloadedEntryId, PostDate = goodsUnloadedEntryPostDate }
                };

                // Goods reserved:

                var goodsReservedEntryId = Guid.NewGuid();
                var goodsReservedEntryPostDate = DateTimeOffset.UtcNow;
                var reservationId = Guid.Parse("7adb94c8-5cee-4f2c-abbf-6d993d2c693f");

                var goodsReserved = new PostGeneralLedgerEntry
                {
                    CreatedOn = DateTimeOffset.UtcNow,
                    PostDate = goodsReservedEntryPostDate,
                    GeneralLedgerEntryId = goodsReservedEntryId,
                    BusinessTransaction = new GoodsReservedTransaction
                    {
                        ReferenceNumber = 1,
                        CustomerId = customerId,
                        LocationId = locationId,
                        SkuId = skuId,
                        Amount = 10,
                        ReservationId = reservationId
                    }
                };

                // Should be translated into:
                var goodsReservedEvents = new object[]
                {
                    new CreditApplied { Account = $"C|{customerId}:WL|{locationId}", GeneralLedgerEntryId = goodsReservedEntryId, Amount = 10, SkuId = skuId },
                    new DebitApplied { Account = $"C|{customerId}:WL|{locationId}:R|{reservationId}", GeneralLedgerEntryId = goodsReservedEntryId, Amount = 10, SkuId = skuId },
                    goodsReserved.BusinessTransaction.GetAdditionalChanges(),
                    new GeneralLedgerEntryPosted { GeneralLedgerEntryId = goodsReservedEntryId, PostDate = goodsReservedEntryPostDate }
                };

                // Goods loaded:
                var goodsLoadedEntryId = Guid.NewGuid();
                var goodsLoadedEntryPostDate = DateTimeOffset.UtcNow;
                var outboundDeliveryId = Guid.Parse("cdc60e2f-77e8-4f37-b9f5-da0ff962453a");

                var goodsLoaded = new PostGeneralLedgerEntry
                {
                    CreatedOn = DateTimeOffset.UtcNow,
                    PostDate = goodsLoadedEntryPostDate,
                    GeneralLedgerEntryId = goodsLoadedEntryId,
                    BusinessTransaction = new GoodsLoadedTransaction
                    {
                        ReferenceNumber = 1,
                        CustomerId = customerId,
                        LocationId = locationId,
                        SkuId = skuId,
                        Amount = 10,
                        ReservationId = reservationId,
                        OutboundDeliveryId = outboundDeliveryId
                    }
                };

                // Should be translated into:
                var goodsLoadedEvents = new object[]
                {
                    new CreditApplied { Account = $"C|{customerId}:WL|{locationId}:R|{reservationId}", GeneralLedgerEntryId = goodsLoadedEntryId, Amount = 10, SkuId = skuId },
                    new DebitApplied { Account = $"C|{customerId}:OD|{outboundDeliveryId}", GeneralLedgerEntryId = goodsLoadedEntryId, Amount = 10, SkuId = skuId },
                    goodsLoaded.BusinessTransaction.GetAdditionalChanges(),
                    new GeneralLedgerEntryPosted { GeneralLedgerEntryId = goodsLoadedEntryId, PostDate = goodsLoadedEntryPostDate }
                };

                // Goods shifted:
                var goodsShiftedEntryId = Guid.NewGuid();
                var goodsShiftedEntryPostDate = DateTimeOffset.UtcNow;
                var destinationLocationId = Guid.Parse("cdc60e2f-77e8-4f37-b9f5-da0ff962453a");

                var goodsShifted = new PostGeneralLedgerEntry
                {
                    CreatedOn = DateTimeOffset.UtcNow,
                    PostDate = goodsShiftedEntryPostDate,
                    GeneralLedgerEntryId = goodsShiftedEntryId,
                    BusinessTransaction = new GoodsShiftedTransaction
                    {
                        ReferenceNumber = 1,
                        CustomerId = customerId,
                        LocationId = locationId,
                        SkuId = skuId,
                        Amount = 10,
                        DestinationLocationId = destinationLocationId
                    }
                };

                // Should be translated into:
                var goodsShiftedEvents = new object[]
                {
                    new CreditApplied { Account = $"C|{customerId}:WL|{locationId}", GeneralLedgerEntryId = goodsShiftedEntryId, Amount = 10, SkuId = skuId },
                    new DebitApplied { Account = $"C|{customerId}:WL|{destinationLocationId}", GeneralLedgerEntryId = goodsShiftedEntryId, Amount = 10, SkuId = skuId },
                    goodsShifted.BusinessTransaction.GetAdditionalChanges(),
                    new GeneralLedgerEntryPosted { GeneralLedgerEntryId = goodsShiftedEntryId, PostDate = goodsShiftedEntryPostDate }
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

                    //DeliveryScheduled
                    connection.AppendToStreamAsync(
                        $"ledgerEntry-{deliveryScheduledEntryId}",
                        ExpectedVersion.Any,
                        deliveryScheduledEvents.Select(@event => new EventData(
                            Guid.NewGuid(),
                            @event.GetType().FullName,
                            true,
                            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)),
                            new byte[0])).ToArray()).GetAwaiter().GetResult();

                    //GoodsUnloaded
                    connection.AppendToStreamAsync(
                        $"ledgerEntry-{goodsUnloadedEntryId}",
                        ExpectedVersion.Any,
                        goodsUnloadedEvents.Select(@event => new EventData(
                            Guid.NewGuid(),
                            @event.GetType().FullName,
                            true,
                            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)),
                            new byte[0])).ToArray()).GetAwaiter().GetResult();

                    //GoodsReserved
                    connection.AppendToStreamAsync(
                        $"ledgerEntry-{goodsReservedEntryId}",
                        ExpectedVersion.Any,
                        goodsReservedEvents.Select(@event => new EventData(
                            Guid.NewGuid(),
                            @event.GetType().FullName,
                            true,
                            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)),
                            new byte[0])).ToArray()).GetAwaiter().GetResult();

                    //GoodsLoaded
                    connection.AppendToStreamAsync(
                        $"ledgerEntry-{goodsLoadedEntryId}",
                        ExpectedVersion.Any,
                        goodsLoadedEvents.Select(@event => new EventData(
                            Guid.NewGuid(),
                            @event.GetType().FullName,
                            true,
                            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)),
                            new byte[0])).ToArray()).GetAwaiter().GetResult();

                    //GoodsShifted
                    connection.AppendToStreamAsync(
                        $"ledgerEntry-{goodsShiftedEntryId}",
                        ExpectedVersion.Any,
                        goodsShiftedEvents.Select(@event => new EventData(
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
