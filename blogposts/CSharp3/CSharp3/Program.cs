using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharp3
{
    class Program
    {
        static void Main(string[] args)
        {
            Product MyProduct = new Product { Name = "Auto", Id = 123  };
            
            List<Product> MyProductList = new List<Product>
            {
                new Product { Name = "Brett", Id = 1 },
                new Product { Name = "Bett", Id = 2 },
                new Product { Name = "Pfanne", Id = 3 }
            };

            var MyVarString = "Something...";
            var MyVarInt = 13;

            var MyVarComplex = new { 
                                    Product = new Product { Name = "Kette", Id = 33},
                                    AnotherProduct = MyProduct,
                                    SimilarProducts = MyProductList,
                                    Information = "This is an additional information.",
                                    SpecialId = MyVarInt,
                                    SpecialString = MyVarString
                                };

            Console.ReadLine();
        }
    }

    public class Product
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
}
