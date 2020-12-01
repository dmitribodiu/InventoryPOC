namespace Goods
{
    public class PackagedProduct
    {
        public string PackagingMaterial { get; set; }
        public int Quantity { get; set; }
        public PackagedProductProperties Properties { get; set; }
        public Product Product { get; set; }
        public string NetWeight { get; set; }
    }
}