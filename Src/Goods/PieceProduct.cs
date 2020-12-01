namespace Goods
{
    public class PieceProduct
    {
        public int Quantity { get; set; }
        public PackagedProductProperties Properties { get; set; }
        public Product Product { get; set; }
        public string NetWeight { get; set; }
    }
}