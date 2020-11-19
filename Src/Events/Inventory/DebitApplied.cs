using System;

namespace Events.Inventory
{
    public class DebitApplied 
    {
        public Guid GeneralLedgerEntryId { get; set; }
        public int Amount { get; set; }
        public string Account { get; set; }
        public Guid SkuId { get; set; }
    }
}
