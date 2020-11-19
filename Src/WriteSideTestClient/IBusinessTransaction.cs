using System;
using System.Collections.Generic;
using Events.Inventory;

namespace WriteSideTestClient
{
    public interface IBusinessTransaction
    {
        GeneralLedgerEntryNumber ReferenceNumber { get; }
        IEnumerable<object> GetAdditionalChanges();
    }

    public class DeliveryScheduledTransaction : IBusinessTransaction
    {
        GeneralLedgerEntryNumber IBusinessTransaction.ReferenceNumber =>
            new GeneralLedgerEntryNumber("deliveryScheduled", ReferenceNumber);

        public int ReferenceNumber { get; set; }
        public Guid CustomerId { get; set; }
        public Guid InboundDeliveryId { get; set; }

        public Guid SkuId { get; set; }
        public int Amount { get; set; }

        public DeliveryScheduledTransaction()
        {
            
        }
        
        public IEnumerable<object> GetAdditionalChanges()
        {
            yield return new DeliveryScheduled
            {
                ReferenceNumber = new GeneralLedgerEntryNumber("deliveryScheduled", ReferenceNumber),
                Amount = Amount,
                SkuId = SkuId,
                CustomerId = CustomerId,
                InboundDeliveryId = InboundDeliveryId

        };
        }
    }
}