using System;
using System.Reflection.Metadata.Ecma335;

namespace Goods
{
    public enum UnitOfMeasurement
    {
        Kg,
        Each
    }

    public enum ProductHandling
    {
        InBulk,
        InPieces
    }

    public class Product
    {
        public Guid ProductId { get; set; }
        public string Color { get; set; }
        public string Name { get; set; }
    }

    public class Article
    {
        public Product Product { get; set; }
        public string Configuration { get; set; }
    }

    public class GoodsSpecification
    {
        public BulkGoodsSpecification BulkGoodsSpecification { get; set; }
        public PackedGoodsSpecification PackedGoodsSpecification { get; set; }
        public BundledGoodsSpecification BundledGoodsSpecification { get; set; }
        public PieceGoodsSpecification PieceGoodsSpecification { get; set; }

    }



    public class BundledGoodsSpecification
    {
    }

    public class PackedGoodsSpecification
    {
    }

    public class BulkGoodsSpecification
    {
    }
}
