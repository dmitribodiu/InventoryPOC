using System.Collections.Generic;

namespace Goods
{
    public class PackagingUnit
    {
        public string PackagingMaterial { get; set; }
        public int Quantity { get; set; }
        public List<PackedGoodsContent> Content { get; set; } = new List<PackedGoodsContent>();
    }
}