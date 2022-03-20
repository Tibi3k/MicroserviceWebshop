using BasketService.DAL.DbContext;
using BasketService.Model;
using MongoDB.Driver;

namespace BasketService.DAL
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IMongoCollection<DbBasket> basketsCollection;
        public BasketRepository(IMongoDatabase database) {
            this.basketsCollection = database.GetCollection<DbBasket>("baskets");
        }
        public void AddProductToBasket(Product product, int userId)
        {

            var qBasket = basketsCollection
                .Find(b => b.UserId == userId)
                .FirstOrDefault();

            if (qBasket == null) {
                var newBasket = new DbBasket()
                {
                    UserId = userId,
                    LastEdited = DateTime.Now,
                    Products = new List<DbProduct>(),
                    TotalCost = 0
                };
                basketsCollection.InsertOne(newBasket);
                qBasket = newBasket;
               
            }

            var dbProduct = DbProduct.ToEntity(product);

            var productsSubCollections = qBasket.Products;
            productsSubCollections.Add(dbProduct);

            var result = basketsCollection.UpdateOne(
                filter: b => b.UserId == userId,
                update: Builders<DbBasket>.Update.Set(b => b.Products, productsSubCollections)
                );
            Console.WriteLine($"Modified {result} lines!");
        }


        public UserBasket FindBasketById(string id)
        {
            throw new NotImplementedException();
        }

        public UserBasket FindBasketByUserId(int id)
        {
            throw new NotImplementedException();
        }

        public List<UserBasket> getAllBaskets()
        {
            var baskets = basketsCollection
                .Find(_ => true)
                .ToList();

            return baskets
                .Select(b => DbBasket.ToModel(b))
                .ToList();
        }
    }
}
