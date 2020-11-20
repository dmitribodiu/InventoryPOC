using System;
using System.Collections.Generic;
using System.Text;

namespace Events.Inventory
{
    public class GoodsLoaded
    {
        public GeneralLedgerEntryNumber ReferenceNumber { get; set; }
        public int Amount { get; set; }
        public Guid SkuId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ReservationId { get; set; }
        public Guid LocationId { get; set; }
        public Guid OutboundDeliveryId { get; set; }
    }
}
