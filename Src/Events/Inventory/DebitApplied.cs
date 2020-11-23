using System;
using System.Collections.Generic;

namespace Events.Inventory
{
    public class DebitApplied 
    {
        public Guid GeneralLedgerEntryId { get; set; }
        public int Amount { get; set; }
        public string Account { get; set; }
        public Guid SkuId { get; set; }
        public Dictionary<string, object> SkuMetadata { get; set; } = new Dictionary<string, object>();
    }
}
