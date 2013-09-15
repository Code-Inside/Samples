using System;
using System.ComponentModel.DataAnnotations;

namespace OneAspNetSample.Models
{
    public class Product
    {
        public Guid Id { get; set; }

        [Required, Display(Name = "Product Name"), StringLength(10)]
        public string Name { get; set; }
        public double Price { get; set; }
    }

}