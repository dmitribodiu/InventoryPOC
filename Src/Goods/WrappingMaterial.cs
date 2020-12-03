using System.Collections.Generic;

namespace Goods
{
    public class WrappingMaterial
    {
        public string Material { get; set; }
        public List<PackedGoodsContent> Content { get; set; } = new List<PackedGoodsContent>();
    }
}