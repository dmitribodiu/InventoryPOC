using System;
using System.Collections.Generic;
using System.Text;

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
        public abstract double GetWeight();

        public abstract double GetNetWeight();
    }

    public class Product : Sku
    {
        public Product(Guid id, string description) : base(id, description)
        {
        }

        public override double GetWeight()
        {
            return 2;
        }

        public override double GetNetWeight()
        {
            throw new NotImplementedException();
        }
    }

    public class PackagingMaterial : Sku
    {
        public PackagingMaterial(Guid id, string description) : base(id, description)
        {
        }

        public override double GetWeight()
        {
            return 2;
        }

        public override double GetNetWeight()
        {
            throw new NotImplementedException();
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

        public override double GetWeight()
        {
            double total = 0;
            
            foreach (var keyValuePair in _skuBom)
            {
                total += keyValuePair.Key.GetWeight() * keyValuePair.Value;
            }
            return total;
        }

        public override double GetNetWeight()
        {
            double total = 0;

            foreach (var keyValuePair in _skuBom)
            {
                total += keyValuePair.Key.GetWeight() * keyValuePair.Value;
            }
            return total;
        }
    }
}
