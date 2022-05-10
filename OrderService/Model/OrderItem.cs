using OrderService.DAL.DbModel;

namespace OrderService.Model;

public class OrderItem
{
    public string OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public int TotalCost { get; set; }
    public List<Product> products { get; set; }
}
