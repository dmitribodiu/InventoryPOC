using System;
using System.Text;

namespace WriteSideTestClient
{
    public class PostGeneralLedgerEntry
    {
        public Guid GeneralLedgerEntryId { get; set; }
        public DateTimeOffset PostDate { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public IBusinessTransaction BusinessTransaction { get; set; }
    }
}
