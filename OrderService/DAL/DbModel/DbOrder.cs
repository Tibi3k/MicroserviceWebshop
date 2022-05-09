using MongoDB.Bson;

namespace OrderService.DAL.DbModel;

public class DbOrder
{
    public ObjectId Id { get; set; }
    public List<DbOrderItem> OrderItems { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string UserId { get; set; }
}
