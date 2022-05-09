using BasketService.DAL.DbContext;

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

        public DbBasket ToEntity() {
            return new DbBasket { 
                Id = MongoDB.Bson.ObjectId.Parse(this.Id), 
                UserId = UserId, 
                Products = DbProduct.ToEntity(Products), 
                LastEdited = LastEdited, 
                 TotalCost = TotalCost,
                 Email = Email
            };
        }
    }

}
