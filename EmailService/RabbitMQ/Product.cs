using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService.RabbitMQ;
public class Product
{
    public Product(int id, string name, string description, int price, int quantity, string? category)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        Quantity = quantity;
        Category = category;
    }

    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = string.Empty;
    public int Price { get; set; }
    public int Quantity { get; set; }
    public string? Category { get; set; }
}