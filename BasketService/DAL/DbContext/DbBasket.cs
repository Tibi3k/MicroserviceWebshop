using BasketService.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BasketService.DAL.DbContext;

public class DbBasket
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string UserId { get; set; }
    public string Email { get; set; }
    public ICollection<DbProduct> Products { get; set; } = new List<DbProduct>();
    public DateTime LastEdited { get; set; }
    public int TotalCost { get; set; }

    public UserBasket ToModel() {
        return new UserBasket()
        {
            Id = this.Id.ToString(),
            UserId = this.UserId,
            Products = DbProduct.ToModel(this.Products),
            LastEdited = this.LastEdited,
            TotalCost = this.TotalCost,
            Email = this.Email
        };
    }
}
