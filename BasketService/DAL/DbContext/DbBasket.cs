using BasketService.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BasketService.DAL.DbContext;

public class DbBasket
{
    [BsonId]
    public ObjectId Id { get; set; }
    public int UserId { get; set; }
    public ICollection<DbProduct> Products { get; set; } = new List<DbProduct>();
    public DateTime LastEdited { get; set; }
    public int TotalCost { get; set; }

    public static UserBasket ToModel(DbBasket basket) {
        return new UserBasket()
        {
            Id = basket.Id.ToString(),
            UserId = basket.UserId,
            Products = DbProduct.ToModel(basket.Products)
        };
    }
}
