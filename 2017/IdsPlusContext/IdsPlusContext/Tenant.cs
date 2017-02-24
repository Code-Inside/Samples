using System;
using System.ComponentModel.DataAnnotations;

namespace IdsPlusContext
{
    public class Tenant
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Notes { get; set; }
    }
}