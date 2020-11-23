using System;

namespace Events.Inventory
{
    public class GeneralLedgerEntryCreated
    {
        public Guid GeneralLedgerEntryId { get; set; }
        public string Number { get; set; } 
        public DateTimeOffset CreatedOn { get; set; }
    }
}
