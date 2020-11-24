using System;
using System.Collections.Generic;
using System.Linq;

namespace Events.Sku
{
    public abstract class Sku
    {
        public Guid Id { get; }
        protected string Description { get; }

        public Sku(Guid id, string description)
        {
            Id = id;
            Description = description;
        }

        public abstract double GetNetWeight();
    }

    public class Product : Sku
    {
        public double NetWeight { get; }

        public Product(Guid id, string description, double netWeight) : base(id, description)
        {
            NetWeight = netWeight;
        }

        public override double GetNetWeight()
        {
            return NetWeight;
        }
    }

    public class PackagingMaterial : Sku
    {
        public double NetWeight { get; }

        public PackagingMaterial(Guid id, string description, double netWeight) : base(id, description)
        {
            NetWeight = netWeight;
        }

        public override double GetNetWeight()
        {
            return NetWeight;
        }
    }

    public class CompositeSku : Sku
    {
        private Dictionary<Sku, int> _skuBom;

        public CompositeSku(Guid id, string description) : base(id, description)
        {
            _skuBom = new Dictionary<Sku, int>();
        }

        public void Add(Sku sku, int ratio)
        {
            _skuBom.Add(sku, ratio);
        }
        public void Remove(Sku sku)
        {
            _skuBom.Remove(sku);
        }

        public override double GetNetWeight()
        {
            double total = 0;

            foreach (var keyValuePair in _skuBom.Where(x=>!(x.Key is PackagingMaterial)))
            {
                total += keyValuePair.Key.GetNetWeight() * keyValuePair.Value;
            }
            return total;
        }
    }
}
