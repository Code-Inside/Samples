using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace GuidVsId
{
    class Program
    {
        static void Main(string[] args)
        {
            // Der NonGUID Part
            Console.WriteLine("NO GUID");
            
            DataClassesNoGuidDataContext noGuidContext = new DataClassesNoGuidDataContext();
           
            NoGuidProduct product = new NoGuidProduct();
            product.Name = "Tolles Produkt";
            product.Price = 14;
            noGuidContext.NoGuidProducts.InsertOnSubmit(product);
            noGuidContext.SubmitChanges();

            Console.WriteLine("Auslesen");
            Table<NoGuidProduct> noguidProductTable = noGuidContext.GetTable<NoGuidProduct>();
            var noguidProducts = from noguidP in noguidProductTable
                               select noguidP;

            foreach (NoGuidProduct resultProduct in noguidProducts)
            {
                Console.WriteLine(resultProduct.Name + " (" + resultProduct.Id + " )");
            }


            

            // Der GUID Part
            DataClassesGuidDataContext guidContext = new DataClassesGuidDataContext();

            Console.WriteLine("GUID");

            GuidProduct guidProduct = new GuidProduct();
            guidProduct.Id = Guid.NewGuid();
            guidProduct.Name = "Tolles Guid Produkt";
            guidProduct.Price = 313;

            GuidProduct anotherGuidProduct = new GuidProduct();
            anotherGuidProduct.Id = Guid.NewGuid();
            anotherGuidProduct.Name = "Tolles anderes Guid Produkt";
            anotherGuidProduct.Price = 132;
            
            GuidReleatedProduct relation = new GuidReleatedProduct();
            relation.Id = Guid.NewGuid();
            relation.ProductIdFirst = guidProduct.Id;
            relation.ProductIdSecond = anotherGuidProduct.Id;

            guidContext.GuidProducts.InsertOnSubmit(guidProduct);
            guidContext.GuidProducts.InsertOnSubmit(anotherGuidProduct);
            guidContext.GuidReleatedProducts.InsertOnSubmit(relation);

            guidContext.SubmitChanges();

            Console.WriteLine("Auslesen");
            
            Table<GuidProduct> guidProductTable = guidContext.GetTable<GuidProduct>();
            var guidProducts = from guidP in guidProductTable
                               select guidP;

            foreach (GuidProduct resultProduct in guidProducts)
            {
                Console.WriteLine(resultProduct.Name);
                foreach(GuidReleatedProduct relatedProducts in resultProduct.GuidReleatedProductsFirst)
                {
                    Console.WriteLine(" - " + relatedProducts.GuidProduct.Name);
                }
            }

            Console.ReadLine();
        }
    }
}
