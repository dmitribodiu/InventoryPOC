using System;
using System.Linq;

namespace Events.Account
{
    public abstract class AccountComponent
    {
        public abstract string GetPrefix();
        public abstract Guid GetId();

        public abstract bool TryParse(string accountComponent, out AccountComponent component);
    }

    public class CustomerComponent : AccountComponent
    {
        public Guid CustomerId { get; set; }
        public const string Prefix = "C";

        public override string GetPrefix()
        {
            return Prefix;
        }

        public override Guid GetId()
        {
            return CustomerId;
        }

        public override bool TryParse(string accountComponent, out AccountComponent component)
        {
            component = null;
            var prefix = accountComponent.Split("|").First();
            if (Prefix == prefix)
            {
                component = new CustomerComponent { CustomerId = Guid.Parse((ReadOnlySpan<char>) accountComponent.Split("|").Last())};
                return true;
            }
            
            return false;
        }

        
    }

    public class InboundDeliveryComponent : AccountComponent
    {
        public Guid InboundDeliveryId { get; set; }
        public const string Prefix = "ID";

        public override string GetPrefix()
        {
            return Prefix;
        }
        public override Guid GetId()
        {
            return InboundDeliveryId;
        }

        public override bool TryParse(string accountComponent, out AccountComponent component)
        {
            component = null;
            var prefix = accountComponent.Split("|").First();
            if (Prefix == prefix)
            {
                component = new InboundDeliveryComponent { InboundDeliveryId = Guid.Parse((ReadOnlySpan<char>)accountComponent.Split("|").Last()) };
                return true;
            }

            return false;
        }
    }

    public class WarehouseLocationComponent : AccountComponent
    {
        public Guid LocationId { get; set; }
        public const string Prefix = "WL";

        public override string GetPrefix()
        {
            return Prefix;
        }
        public override Guid GetId()
        {
            return LocationId;
        }

        public override bool TryParse(string accountComponent, out AccountComponent component)
        {
            component = null;
            var prefix = accountComponent.Split("|").First();
            if (Prefix == prefix)
            {
                component = new WarehouseLocationComponent { LocationId = Guid.Parse((ReadOnlySpan<char>)accountComponent.Split("|").Last()) };
                return true;
            }

            return false;
        }
    }

    public class ReservationComponent : AccountComponent
    {
        public Guid ReservationId { get; set; }
        public const string Prefix = "R";

        public override string GetPrefix()
        {
            return Prefix;
        }
        public override Guid GetId()
        {
            return ReservationId;
        }

        public override bool TryParse(string accountComponent, out AccountComponent component)
        {
            component = null;
            var prefix = accountComponent.Split("|").First();
            if (Prefix == prefix)
            {
                component = new ReservationComponent { ReservationId = Guid.Parse((ReadOnlySpan<char>)accountComponent.Split("|").Last()) };
                return true;
            }

            return false;
        }
    }

    public class OutboundDeliveryComponent : AccountComponent
    {
        public Guid OutboundDeliveryId { get; set; }
        public const string Prefix = "OD";

        public override string GetPrefix()
        {
            return Prefix;
        }
        public override Guid GetId()
        {
            return OutboundDeliveryId;
        }

        public override bool TryParse(string accountComponent, out AccountComponent component)
        {
            component = null;
            var prefix = accountComponent.Split("|").First();
            if (Prefix == prefix)
            {
                component = new OutboundDeliveryComponent { OutboundDeliveryId = Guid.Parse((ReadOnlySpan<char>)accountComponent.Split("|").Last()) };
                return true;
            }

            return false;
        }
    }
    public class HandlingUnitComponent : AccountComponent
    {
        public Guid Id { get; set; }
        public Guid HandlingUnitId { get; set; }

        public const string Prefix = "HU";

        public override string GetPrefix()
        {
            return Prefix;
        }
        public override Guid GetId()
        {
            return Id;
        }

        public override bool TryParse(string accountComponent, out AccountComponent component)
        {
            component = null;
            var prefix = accountComponent.Split("|").First();
            if (Prefix == prefix)
            {
                component = new HandlingUnitComponent
                {
                    Id = Guid.Parse((ReadOnlySpan<char>)accountComponent.Split("|").Last().Split(",").First()),
                    HandlingUnitId = Guid.Parse((ReadOnlySpan<char>)accountComponent.Split("|").Last().Split(",").Last())
                };
                return true;
            }

            return false;
        }
    }
}