using System;
using System.Collections.Generic;
using Goods;

namespace NewSchemaExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            var coffee = new Product { ProductId = Guid.NewGuid(), Name = "Coffee", Color = "Black"};
            var tea = new Product { ProductId = Guid.NewGuid(), Name = "Tea", Color = "Green" };
            var cp7Pallet = new Product { ProductId = Guid.NewGuid(), Name = "CP7", Color = "NONE" };

            var packedGoodsExample = new PackedGoodsSpecification
            {
                BasedOnArticle = new Article { Configuration = "55bags-25kg-cp7", Product = coffee },
                TransportUnit = new TransportHandlingUnit
                {
                    Material = "CP7",
                    Properties = new TransportHandlingUnitProperties { PalletProperties = new PalletTransportHandlingUnitProperties
                    {
                        PalletNumber = "NONE"
                    } },
                    Contents = new List<PackedGoodsContent>{new  PackedGoodsContent
                    {
                        PackagedProduct = new PackagedProduct
                        {
                            PackagingMaterial = "25kg bag",
                            Quantity = 55,
                            Properties = new PackagedProductProperties {  Batch = "A", SerialNumber = "NONE"},
                            Product = coffee,
                            NetWeight = "25kg"
                        }
                    }}
                }
            };
        }
    }
}
