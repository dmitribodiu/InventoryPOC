namespace Goods
{
    public class AnonymusPackagedProduct
    {
        public string PackagingMaterial { get; set; }
        public int Quantity { get; set; }
        public AnonymousProductHazardousDocument AnonymousProductHazardousDocument { get; set; }
    }

    public class AnonymousProductHazardousDocument
    {
        public string Uncode { get; set; }
        public string FlashPoint { get; set; }
        public bool MarinePollutant { get; set; }
    }
}