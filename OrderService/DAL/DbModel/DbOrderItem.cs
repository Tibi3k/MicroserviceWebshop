using MongoDB.Bson;

namespace OrderService.DAL.DbModel;

public class DbOrderItem
{
    public string OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public int TotalCost { get; set; }
    public List<Product> products { get; set; }

}
