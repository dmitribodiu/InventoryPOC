using System;

namespace Events.Inventory
{
    public class GoodsUnloaded
    {
        public GeneralLedgerEntryNumber ReferenceNumber { get; set; }
        public int Amount { get; set; }
        public Guid SkuId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid InboundDeliveryId { get; set; }
        public Guid WorkOrderId { get; set; }
        public Guid LocationId { get; set; }
    }
}
