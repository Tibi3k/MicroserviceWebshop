namespace OrderService.Model;

public class Order
{
    public string Id { get; set; }
    public List<OrderItem> OrderItems { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string UserId { get; set; }
}

