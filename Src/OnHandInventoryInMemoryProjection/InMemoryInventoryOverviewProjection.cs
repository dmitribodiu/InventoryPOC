using System;
using Events.Account;
using Events.Inventory;
using Microsoft.Extensions.Caching.Memory;
using Projac;

namespace OnHandInventoryInMemoryProjection
{
    public class InMemoryInventoryOverviewProjection
    {
        public static AnonymousProjection<MemoryCache> Projection =
            new AnonymousProjectionBuilder<MemoryCache>().
                When<DebitApplied>((Action<MemoryCache, DebitApplied>) DebitApplied).
                When<CreditApplied>((Action<MemoryCache, CreditApplied>)CreditApplied).
                Build();

        private static void DebitApplied(MemoryCache cache, DebitApplied message)
        {
            var account = Account.Parse(message.Account);

            if (!account.ContainsComponent<WarehouseLocationComponent>()) return;
            
            var accountId = account.GetId();
            var location = account.GetComponent<WarehouseLocationComponent>();
            var reservation = account.TryGetComponent<ReservationComponent>();

            var id = StockLinePartId.NewId(message.SkuId, accountId, message.SkuMetadata);

            message.SkuMetadata.TryGetValue("Batch", out var batchValue);

            if (cache.TryGetValue(id, out StockLine stockLine))
            {
                stockLine.Amount += message.Amount;
            }
            else
            {
                cache.Set(id,
                    new StockLine
                    {
                        SkuId = message.SkuId,
                        Amount = message.Amount,
                        LocationId = location.LocationId,
                        ReservationId = reservation?.ReservationId,
                        Batch = Convert.ToString(batchValue)
                    });
            }
        }

        private static void CreditApplied(MemoryCache cache, CreditApplied message)
        {
            var account = Account.Parse(message.Account);
            if (!account.ContainsComponent<WarehouseLocationComponent>()) return;

            var accountId = account.GetId();

            var id = StockLinePartId.NewId(message.SkuId, accountId, message.SkuMetadata);

            if (cache.TryGetValue(id, out StockLine stockLine))
            {
                stockLine.Amount -= message.Amount;

                if (stockLine.Amount == 0)
                {
                    cache.Remove(id);
                }
            }
        }
    }
}
