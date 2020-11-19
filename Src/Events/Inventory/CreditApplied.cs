using System;

namespace Events.Inventory
{
    public class CreditApplied
    {
        public Guid GeneralLedgerEntryId { get; set; }
        public decimal Amount { get; set; }
        public Guid SkuId { get; set; }
        public string Account { get; set; }
    }
}
