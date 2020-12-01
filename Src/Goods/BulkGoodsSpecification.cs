namespace Goods
{
    public class BulkGoodsSpecification
    {
        public Article BasedOnArticle { get; set; }
        public Product Product { get; set; }
        public string NetWeight { get; set; }
        public BulkGoodsProperties BulkGoodsProperties { get; set; }
    }

    public class BulkGoodsProperties
    {
        public string Batch { get; set; }
    }
}