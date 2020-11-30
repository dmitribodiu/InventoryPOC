using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Events;
using Events.Inventory;
using Events.Sku;
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
                    new GeneralLedgerEntryCreated { GeneralLedgerEntryId = deliveryScheduledEntryId, Number = deliveryScheduled.BusinessTransaction.ReferenceNumber.ToString(), CreatedOn = deliveryScheduled.CreatedOn },
                    new CreditApplied { Account = $"C|{customerId}", GeneralLedgerEntryId = deliveryScheduledEntryId, Amount = 20, SkuId = skuId },
                    new DebitApplied { Account = $"C|{customerId}:ID|{inboundDeliveryId}", GeneralLedgerEntryId = deliveryScheduledEntryId, Amount = 20, SkuId = skuId },
                    deliveryScheduled.BusinessTransaction.GetAdditionalChanges().Single(),
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

                // lets imagine 20 pallets are unloaded (BatchA) + one partial unit (54 bags) (BatchA) + one mixed unit (25 BatchA, 30 BatchB) + 1 variable (540 kg).
                // They all go to the same location for now.

                var oneKgOfCoffee = new Product(Guid.NewGuid(), "Coffee 'Jacobs', 1 kg", 1);
                var onePolyBag = new PackagingMaterial(Guid.NewGuid(), "Poly bag, Volume: 25 kg", 0.2);
                var oneBag = new CompositeSku(Guid.NewGuid(), "One bag of 25 kg of 'Jacobs' coffee");
                oneBag.Add(onePolyBag, 1);
                oneBag.Add(oneKgOfCoffee, 25);

                var pallet = new PackagingMaterial(Guid.NewGuid(), "Pallet, CP7", 17);

                var palletWith55Bags = new CompositeSku(Guid.NewGuid(), "Pallet with 55 bags of coffee, 25 kg in each");
                palletWith55Bags.Add(pallet, 1);
                palletWith55Bags.Add(oneBag, 55);

                var mixedPalletId = Guid.NewGuid();

                // Should be translated into:
                var goodsUnloadedEvents = new object[]
                {
                    //Skus should be pushed to a separate stream, when Article is created.
                    new SkuDefined { Sku = oneKgOfCoffee },
                    new SkuDefined { Sku = oneBag },
                    new SkuDefined { Sku = palletWith55Bags },
                    new GeneralLedgerEntryCreated { GeneralLedgerEntryId = goodsUnloadedEntryId, Number = goodsUnloaded.BusinessTransaction.ReferenceNumber.ToString(), CreatedOn = goodsUnloaded.CreatedOn },
                    new CreditApplied { Account = $"C|{customerId}:ID|{inboundDeliveryId}", GeneralLedgerEntryId = goodsUnloadedEntryId, Amount = 20, SkuId = skuId },
                    new DebitApplied { Account = $"C|{customerId}:WL|{locationId}", GeneralLedgerEntryId = goodsUnloadedEntryId, Amount = 20, SkuId = palletWith55Bags.Id,
                        SkuMetadata = new Dictionary<string, object>{ {"Batch", "A"}}},
                    new DebitApplied { Account = $"C|{customerId}:WL|{locationId}:HU|{Guid.NewGuid()},{pallet.Id}", GeneralLedgerEntryId = goodsUnloadedEntryId, Amount = 54, SkuId = oneBag.Id,
                        SkuMetadata = new Dictionary<string, object>{ {"Batch", "A"}}},
                    new DebitApplied { Account = $"C|{customerId}:WL|{locationId}:HU|{mixedPalletId},{pallet.Id}", GeneralLedgerEntryId = goodsUnloadedEntryId, Amount = 25, SkuId = oneBag.Id,
                        SkuMetadata = new Dictionary<string, object>{ {"Batch", "A"}}},
                    new DebitApplied { Account = $"C|{customerId}:WL|{locationId}:HU|{mixedPalletId},{pallet.Id}", GeneralLedgerEntryId = goodsUnloadedEntryId, Amount = 30, SkuId = oneBag.Id,
                        SkuMetadata = new Dictionary<string, object>{ {"Batch", "B"}}},
                    new DebitApplied { Account = $"C|{customerId}:WL|{locationId}:HU|{Guid.NewGuid()},{pallet.Id}:HU|{Guid.NewGuid()},{oneBag.Id}", GeneralLedgerEntryId = goodsUnloadedEntryId, Amount = 540, SkuId = oneKgOfCoffee.Id,
                        SkuMetadata = new Dictionary<string, object>{ {"Batch", "B"}}},
                    goodsUnloaded.BusinessTransaction.GetAdditionalChanges().Single(),
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
                        SkuId = palletWith55Bags.Id,
                        Amount = 10,
                        ReservationId = reservationId
                    }
                };

                // Should be translated into:
                var goodsReservedEvents = new object[]
                {
                    new GeneralLedgerEntryCreated { GeneralLedgerEntryId = goodsReservedEntryId, Number = goodsReserved.BusinessTransaction.ReferenceNumber.ToString(), CreatedOn = goodsReserved.CreatedOn },
                    new CreditApplied { Account = $"C|{customerId}:WL|{locationId}", GeneralLedgerEntryId = goodsReservedEntryId, Amount = 10, SkuId = palletWith55Bags.Id, SkuMetadata = new Dictionary<string, object>{ {"Batch", "A"}} },
                    new DebitApplied { Account = $"C|{customerId}:WL|{locationId}:R|{reservationId}", GeneralLedgerEntryId = goodsReservedEntryId, Amount = 10, SkuId = palletWith55Bags.Id, SkuMetadata = new Dictionary<string, object>{ {"Batch", "A"}} },
                    goodsReserved.BusinessTransaction.GetAdditionalChanges().Single(),
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
                    new GeneralLedgerEntryCreated { GeneralLedgerEntryId = goodsLoadedEntryId, Number = goodsLoaded.BusinessTransaction.ReferenceNumber.ToString(), CreatedOn = goodsLoaded.CreatedOn },
                    new CreditApplied { Account = $"C|{customerId}:WL|{locationId}:R|{reservationId}", GeneralLedgerEntryId = goodsLoadedEntryId, Amount = 10, SkuId = palletWith55Bags.Id, SkuMetadata = new Dictionary<string, object>{ {"Batch", "A"}} },
                    new DebitApplied { Account = $"C|{customerId}:OD|{outboundDeliveryId}", GeneralLedgerEntryId = goodsLoadedEntryId, Amount = 10, SkuId = palletWith55Bags.Id, SkuMetadata = new Dictionary<string, object>{ {"Batch", "A"}} },
                    goodsLoaded.BusinessTransaction.GetAdditionalChanges().Single(),
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
                    new GeneralLedgerEntryCreated { GeneralLedgerEntryId = goodsShiftedEntryId, Number = goodsShifted.BusinessTransaction.ReferenceNumber.ToString(), CreatedOn = goodsShifted.CreatedOn },
                    new CreditApplied { Account = $"C|{customerId}:WL|{locationId}", GeneralLedgerEntryId = goodsShiftedEntryId, Amount = 10, SkuId = palletWith55Bags.Id, SkuMetadata = new Dictionary<string, object>{ {"Batch", "A"}} },
                    new DebitApplied { Account = $"C|{customerId}:WL|{destinationLocationId}", GeneralLedgerEntryId = goodsShiftedEntryId, Amount = 10, SkuId = palletWith55Bags.Id, SkuMetadata = new Dictionary<string, object>{ {"Batch", "A"}} },
                    goodsShifted.BusinessTransaction.GetAdditionalChanges().Single(),
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

                var jsonSerializerSettings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new DictionaryAsArrayResolver()
                };

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
                            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event, jsonSerializerSettings)),
                            new byte[0])).ToArray()).GetAwaiter().GetResult();

                    //GoodsUnloaded
                    connection.AppendToStreamAsync(
                        $"ledgerEntry-{goodsUnloadedEntryId}",
                        ExpectedVersion.Any,
                        goodsUnloadedEvents.Select(@event => new EventData(
                            Guid.NewGuid(),
                            @event.GetType().FullName,
                            true,
                            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event, jsonSerializerSettings)),
                            new byte[0])).ToArray()).GetAwaiter().GetResult();

                    //GoodsReserved
                    connection.AppendToStreamAsync(
                        $"ledgerEntry-{goodsReservedEntryId}",
                        ExpectedVersion.Any,
                        goodsReservedEvents.Select(@event => new EventData(
                            Guid.NewGuid(),
                            @event.GetType().FullName,
                            true,
                            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event, jsonSerializerSettings)),
                            new byte[0])).ToArray()).GetAwaiter().GetResult();

                    //GoodsLoaded
                    connection.AppendToStreamAsync(
                        $"ledgerEntry-{goodsLoadedEntryId}",
                        ExpectedVersion.Any,
                        goodsLoadedEvents.Select(@event => new EventData(
                            Guid.NewGuid(),
                            @event.GetType().FullName,
                            true,
                            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event, jsonSerializerSettings)),
                            new byte[0])).ToArray()).GetAwaiter().GetResult();

                    //GoodsShifted
                    connection.AppendToStreamAsync(
                        $"ledgerEntry-{goodsShiftedEntryId}",
                        ExpectedVersion.Any,
                        goodsShiftedEvents.Select(@event => new EventData(
                            Guid.NewGuid(),
                            @event.GetType().FullName,
                            true,
                            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event, jsonSerializerSettings)),
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
