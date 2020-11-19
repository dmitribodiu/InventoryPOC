using System;
using System.Collections.Generic;

namespace Events.Inventory
{
    public class DeliveryScheduled
    {
        public GeneralLedgerEntryNumber ReferenceNumber { get; set; }
        public Guid CustomerId { get; set; }
        public Guid InboundDeliveryId { get; set; }

        public Guid SkuId { get; set; }
        public int Amount { get; set; }

    }
}
