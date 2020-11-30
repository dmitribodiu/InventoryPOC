using System;
using System.Collections.Generic;
using System.Linq;

namespace Events.Sku
{
    public abstract class Sku
    {
        public Guid Id { get; set; }
        public string Description { get; set; }

        public Sku(Guid id, string description)
        {
            Id = id;
            Description = description;
        }

        public Sku()
        {
            
        }

        public abstract double GetNetWeight();
    }

    public class Product : Sku
    {
        public double NetWeight { get; set; }

        public Product()
        {
            
        }
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
        public double NetWeight { get; set; }

        public PackagingMaterial()
        {
            
        }
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
        public Dictionary<Sku, int> SkuBom { get; set; }

        public CompositeSku(Guid id, string description) : base(id, description)
        {
            SkuBom = new Dictionary<Sku, int>();
        }

        public CompositeSku()
        {
            
        }
        public void Add(Sku sku, int ratio)
        {
            SkuBom.Add(sku, ratio);
        }
        public void Remove(Sku sku)
        {
            SkuBom.Remove(sku);
        }

        public override double GetNetWeight()
        {
            double total = 0;

            foreach (var keyValuePair in SkuBom.Where(x=>!(x.Key is PackagingMaterial)))
            {
                total += keyValuePair.Key.GetNetWeight() * keyValuePair.Value;
            }
            return total;
        }
    }
}
