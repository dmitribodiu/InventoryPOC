using System;
using System.Collections.Generic;
using Events.Inventory;

namespace WriteSideTestClient
{
    public interface IBusinessTransaction
    {
        GeneralLedgerEntryNumber ReferenceNumber { get; }
        IEnumerable<object> GetAdditionalChanges();
    }

    public class DeliveryScheduledTransaction : IBusinessTransaction
    {
        GeneralLedgerEntryNumber IBusinessTransaction.ReferenceNumber =>
            new GeneralLedgerEntryNumber("deliveryScheduled", ReferenceNumber);

        public int ReferenceNumber { get; set; }
        public Guid CustomerId { get; set; }
        public Guid InboundDeliveryId { get; set; }

        public Guid SkuId { get; set; }
        public int Amount { get; set; }

        public DeliveryScheduledTransaction()
        {
            
        }
        
        public IEnumerable<object> GetAdditionalChanges()
        {
            yield return new DeliveryScheduled
            {
                ReferenceNumber = new GeneralLedgerEntryNumber("deliveryScheduled", ReferenceNumber),
                Amount = Amount,
                SkuId = SkuId,
                CustomerId = CustomerId,
                InboundDeliveryId = InboundDeliveryId

        };
        }
    }

    public class GoodsArrivedOnSiteTransaction : IBusinessTransaction
    {
        GeneralLedgerEntryNumber IBusinessTransaction.ReferenceNumber =>
            new GeneralLedgerEntryNumber("arrivedOnSite", ReferenceNumber);

        public int ReferenceNumber { get; set; }
        public Guid CustomerId { get; set; }
        public Guid InboundDeliveryId { get; set; }
        public Guid SkuId { get; set; }
        public int Amount { get; set; }

        public IEnumerable<object> GetAdditionalChanges()
        {
            yield return new DeliveryScheduled
            {
                ReferenceNumber = new GeneralLedgerEntryNumber("arrivedOnSite", ReferenceNumber),
                Amount = Amount,
                SkuId = SkuId,
                CustomerId = CustomerId,
                InboundDeliveryId = InboundDeliveryId

            };
        }
    }

    public class GoodsUnloadedTransaction : IBusinessTransaction
    {
        GeneralLedgerEntryNumber IBusinessTransaction.ReferenceNumber =>
            new GeneralLedgerEntryNumber("goodsUnloaded", ReferenceNumber);

        public int ReferenceNumber { get; set; }
        public Guid CustomerId { get; set; }
        public Guid InboundDeliveryId { get; set; }
        public Guid WorkOrderId { get; set; }
        public Guid SkuId { get; set; }
        public int Amount { get; set; }
        public Guid LocationId { get; set; }

        public IEnumerable<object> GetAdditionalChanges()
        {
            yield return new GoodsUnloaded
            {
                ReferenceNumber = new GeneralLedgerEntryNumber("goodsUnloaded", ReferenceNumber),
                Amount = Amount,
                SkuId = SkuId,
                CustomerId = CustomerId,
                InboundDeliveryId = InboundDeliveryId,
                WorkOrderId = WorkOrderId,
                LocationId = LocationId
            };
        }
    }

    public class GoodsReservedTransaction : IBusinessTransaction
    {
        GeneralLedgerEntryNumber IBusinessTransaction.ReferenceNumber =>
            new GeneralLedgerEntryNumber("goodsReserved", ReferenceNumber);

        public int ReferenceNumber { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ReservationId { get; set; }
        public Guid SkuId { get; set; }
        public int Amount { get; set; }
        public Guid LocationId { get; set; }

        public IEnumerable<object> GetAdditionalChanges()
        {
            yield return new GoodsReserved
            {
                ReferenceNumber = new GeneralLedgerEntryNumber("goodsReserved", ReferenceNumber),
                Amount = Amount,
                SkuId = SkuId,
                CustomerId = CustomerId,
                ReservationId = ReservationId,
                LocationId = LocationId
            };
        }
    }

    public class GoodsLoadedTransaction : IBusinessTransaction
    {
        GeneralLedgerEntryNumber IBusinessTransaction.ReferenceNumber =>
            new GeneralLedgerEntryNumber("goodsLoaded", ReferenceNumber);

        public int ReferenceNumber { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ReservationId { get; set; }
        public Guid SkuId { get; set; }
        public int Amount { get; set; }
        public Guid LocationId { get; set; }
        public Guid OutboundDeliveryId { get; set; }

        public IEnumerable<object> GetAdditionalChanges()
        {
            yield return new GoodsLoaded
            {
                ReferenceNumber = new GeneralLedgerEntryNumber("goodsLoaded", ReferenceNumber),
                Amount = Amount,
                SkuId = SkuId,
                CustomerId = CustomerId,
                ReservationId = ReservationId,
                LocationId = LocationId,
                OutboundDeliveryId = OutboundDeliveryId
            };
        }
    }
}