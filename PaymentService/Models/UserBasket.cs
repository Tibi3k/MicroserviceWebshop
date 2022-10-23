using OrderService.DAL.DbModel;

namespace BasketService.Model
{
    public class UserBasket
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public DateTime LastEdited { get; set; }
        public int TotalCost { get; set; }
    }

}
