using System.ComponentModel.DataAnnotations;

namespace ProductService.Model
{
    public class Product
    {
        public Product(int id, string name, string description, int price, int quantity, string category) { 
            Id = id;
            Name = name; 
            Description = description;
            Price = price;
            Quantity = quantity;
            Category = category;
        }

        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = "";

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, 1000000)]
        public int Price { get; set; } 

        [Required]
        [Range(0, 100000)]
        public int Quantity { get; set; }

        public string Category { get; set; }
    }
}
