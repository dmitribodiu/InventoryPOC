namespace Goods
{
    public class PackagingUnit
    {
        public string PackagingMaterial { get; set; }
        public int Quantity { get; set; }
        public PackedGoodsContent Content { get; set; }
    }
}