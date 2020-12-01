namespace Goods
{
    public class PackedGoodsSpecification
    {
        public Article BasedOnArticle { get; set; }
        public TransportHandlingUnit TransportUnit { get; set; }

    }

    public class TransportHandlingUnit
    {
        public string Material { get; set; }
        public TransportHandlingUnitProperties Properties { get; set; }
        public PackedGoodsContent Contents { get; set; }
    }
}