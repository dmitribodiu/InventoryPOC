using System;

namespace Events.Inventory
{
    public class GeneralLedgerEntryPosted
    {
        public Guid GeneralLedgerEntryId { get; set; }
        public DateTimeOffset PostDate { get; set; }
    }
}
