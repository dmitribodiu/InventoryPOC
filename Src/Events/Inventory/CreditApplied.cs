using System;

namespace Events.Inventory
{
    public class CreditApplied 
    {
        public Guid GeneralLedgerEntryId { get; set; }
        public int Amount { get; set; }
        public Guid SkuId { get; set; }
        public string Account { get; set; }
    }
}
