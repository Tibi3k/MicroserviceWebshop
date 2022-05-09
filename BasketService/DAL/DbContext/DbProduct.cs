using BasketService.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BasketService.DAL.DbContext;

public class DbProduct
{
    public DbProduct() { }
    public DbProduct(int Id, string Name, string Description, int Price, int Quantity, string Category)
    {
        this.Id = Id;
        this.Name = Name;
        this.Description = Description;
        this.Price = Price;
        this.Quantity = Quantity;
        this.Category = Category;
    }

    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = string.Empty;
    public int Price { get; set; }
    public int Quantity { get; set; }
    public string Category { get; set; }
    public Guid BasketSubId { get; set; }
    public DateTime AddTime { get; set; }

    public override string ToString()
    {
        return $"Id: {this.Id}, Name: {this.Name}";
    }

    public static List<Product> ToModel(ICollection<DbProduct> products) {
        var list = new List<Product>();
        foreach (var product in products)
        {
            list.Add(ToModel(product));
        }
        return list;
    }
    public static List<DbProduct> ToEntity(ICollection<Product> products)
    {
        var list = new List<DbProduct>();
        foreach (var product in products)
        {
            list.Add(ToEntity(product));
        }
        return list;
    }


    public static DbProduct ToEntity(Product product)
    {
        return new DbProduct
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
            Category = product.Category,
            AddTime = product.AddTime,
            BasketSubId = Guid.Parse(product.BasketSubId)
        };
    }

    public static Product ToModel(DbProduct product)
    {
        return new Product
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
            Category = product.Category,
            AddTime = product.AddTime,
            BasketSubId = product.BasketSubId.ToString()
        };
    }
}
