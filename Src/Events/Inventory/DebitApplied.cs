using System;

namespace Events.Inventory
{
    public class DebitApplied
    {
        public Guid GeneralLedgerEntryId { get; set; }
        public decimal Amount { get; set; }
        public string Account { get; set; }
        public Guid SkuId { get; set; }
    }
}
