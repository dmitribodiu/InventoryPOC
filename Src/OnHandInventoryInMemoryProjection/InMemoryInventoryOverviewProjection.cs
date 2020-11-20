using System;
using Events;
using Events.Inventory;
using Microsoft.Extensions.Caching.Memory;
using Projac;

namespace OnHandInventoryInMemoryProjection
{
    public class InMemoryInventoryOverviewProjection
    {
        public static AnonymousProjection<MemoryCache> Projection =
            new AnonymousProjectionBuilder<MemoryCache>().
                When<DebitApplied>((cache, message) =>
                {
                    cache.Set(Guid.NewGuid(), new StockLine
                    {
                        SkuId = message.SkuId,
                        Amount = message.Amount,
                        LocationId = Guid.NewGuid(),
                        ReservationId = null
                    });
                }).
                When<CreditApplied>((cache, message) =>
                {
                    cache.Set(Guid.NewGuid(), new StockLine
                    {
                        SkuId = message.SkuId,
                        Amount = message.Amount,
                        LocationId = Guid.NewGuid(),
                        ReservationId = null
                    });
                }).
                Build();
    }
}
