namespace Goods
{
    public class BundledGoodsSpecification
    {
        public string BundleNumber { get; set; }
        public BundlePieceProduct Contents { get; set; }
    }

    public class BundlePieceProduct
    {
        public int Quantity { get; set; }
        public Product Product { get; set; }
        public BundledProductProperties Properties { get; set; }
        public string Weight { get; set; }
    }

    public class BundledProductProperties
    {
        public BundledProductDimensions BundledProductDimensions { get; set; }
        public string Grammage { get; set; }
        public string Quality { get; set; }
        public string Standart { get; set; }
        public string Color { get; set; }
        public string Shape { get; set; }
    }

    public class BundledProductDimensions
    {
        public string Length { get; set; }
        public string ThickNess { get; set; }
        public string Diameter { get; set; }
        public string Width { get; set; }
    }
}