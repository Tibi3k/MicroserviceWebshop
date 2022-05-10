using MongoDB.Bson;
using OrderService.Model;

namespace OrderService.DAL.DbModel;

public class DbOrderItem
{
    public string OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public int TotalCost { get; set; }
    public List<Product> products { get; set; }

    public OrderItem toOrder() {
        return new OrderItem
        {
            OrderDate = OrderDate,
            OrderId = OrderId,
            TotalCost = TotalCost,
            products = products
        };
    }

}
