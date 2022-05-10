using MongoDB.Bson;
using OrderService.Model;

namespace OrderService.DAL.DbModel;

public class DbOrder
{
    public ObjectId Id { get; set; }
    public List<DbOrderItem> OrderItems { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string UserId { get; set; }

    public Order toOrder() {
        return new Order
        {
            OrderItems = this.OrderItems
                .Select(oi =>
                    oi.toOrder()
                ).ToList(),
            Id = Id.ToString(),
            Username = Username,
            Email = Email,
            UserId = UserId
        };
    }
}

