namespace Goods
{
    public class PackedGoodsContent
    {
        public WrappingMaterial WrappingMaterial { get; set; }
        public PackagingUnit PackagingUnit { get; set; }
        public PackagedProduct PackagedProduct { get; set; }
        public PieceProduct PieceProduct { get; set; }
        public AnonymusPackagedProduct AnonymusPackagedProduct { get; set; }
    }
}