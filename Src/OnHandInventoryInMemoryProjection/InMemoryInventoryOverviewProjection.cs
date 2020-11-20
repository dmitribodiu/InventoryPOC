using System;
using System.Linq;
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
                When<DebitApplied>((Action<MemoryCache, DebitApplied>) DebitAppliedOnAvailableAccount).
                When<DebitApplied>((Action<MemoryCache, DebitApplied>)DebitAppliedOnReservedAccount).
                When<CreditApplied>((Action<MemoryCache, CreditApplied>)CreditAppliedOnAvailableAccount).
                When<CreditApplied>((Action<MemoryCache, CreditApplied>)CreditAppliedOnReservedAccount).
                Build();

        private static void DebitAppliedOnAvailableAccount(MemoryCache cache, DebitApplied message)
        {
            var lastAccount = message.Account.Split(":").Last();
            var lastAccountPrefix = lastAccount.Split("|").First();
            var locationId = lastAccount.Split("|").Last();
            var locationAsGuid = Guid.Parse(locationId);

            if (lastAccountPrefix != "WL")
            {
                return;
            }

            var id = StockLinePartId.NewId(message.SkuId, locationAsGuid);

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
                        LocationId = locationAsGuid,
                        Amount = message.Amount,
                        ReservationId = null
                    });
            }
        }

        private static void DebitAppliedOnReservedAccount(MemoryCache cache, DebitApplied message)
        {
            var lastAccount = message.Account.Split(":").Last();
            var lastAccountPrefix = lastAccount.Split("|").First();
            var reservationId = lastAccount.Split("|").Last();
            var reservationAsGuid = Guid.Parse(reservationId);

            if (lastAccountPrefix != "R")
            {
                return;
            }

            var penUltimateAccount = message.Account.Split(":").Reverse().Skip(1).First();
            var locationId = penUltimateAccount.Split("|").Last();
            var locationIdAsGuid = Guid.Parse(locationId);

            var id = StockLinePartId.NewId(message.SkuId, locationIdAsGuid, reservationAsGuid);

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
                        LocationId = locationIdAsGuid,
                        Amount = message.Amount,
                        ReservationId = reservationAsGuid
                    });
            }
        }

        private static void CreditAppliedOnAvailableAccount(MemoryCache cache, CreditApplied message)
        {
            var lastAccount = message.Account.Split(":").Last();
            var lastAccountPrefix = lastAccount.Split("|").First();
            var locationId = lastAccount.Split("|").Last();
            var locationAsGuid = Guid.Parse(locationId);

            if (lastAccountPrefix != "WL")
            {
                return;
            }

            var id = StockLinePartId.NewId(message.SkuId, locationAsGuid);

            if (cache.TryGetValue(id, out StockLine stockLine))
            {
                stockLine.Amount -= message.Amount;

                if (stockLine.Amount == 0)
                {
                    cache.Remove(id);
                }
            }
        }

        private static void CreditAppliedOnReservedAccount(MemoryCache cache, CreditApplied message)
        {
            var lastAccount = message.Account.Split(":").Last();
            var lastAccountPrefix = lastAccount.Split("|").First();
            var reservationId = lastAccount.Split("|").Last();
            var reservationAsGuid = Guid.Parse(reservationId);

            if (lastAccountPrefix != "R")
            {
                return;
            }

            var penUltimateAccount = message.Account.Split(":").Reverse().Skip(1).First();
            var locationId = penUltimateAccount.Split("|").Last();
            var locationIdAsGuid = Guid.Parse(locationId);

            var id = StockLinePartId.NewId(message.SkuId, locationIdAsGuid, reservationAsGuid);

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
