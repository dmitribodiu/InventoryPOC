using System;

namespace Goods
{
    public class PieceGoodsSpecification
    {
        public Guid? PieceId { get; set; }
        public Product Product { get; set; }
        public PieceGoodsProperties  PieceGoodsProperties { get; set; }
    }

    public class PieceGoodsProperties
    {
        public PieceGoodsDimensions PieceGoodsDimensions { get; set; }
        public string Grammage { get; set; }
        public string Quality { get; set; }
        public string Standart { get; set; }
    }

    public class PieceGoodsDimensions
    {
        public string Length { get; set; }
        public string Thickness { get; set; }
        public string Diameter { get; set; }
        public string Width { get; set; }
        public string StripLength { get; set; }
    }
}