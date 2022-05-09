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
        public async Task AddProductToBasketAsync(Product product, string userId, string email)
        {

            var qBasket = await basketsCollection.FindAsync(b => b.UserId == userId);
            var userBasket = await qBasket.FirstOrDefaultAsync();

            if (userBasket == null) {
                var newBasket = new DbBasket()
                {
                    UserId = userId,
                    LastEdited = DateTime.Now,
                    Products = new List<DbProduct>(),
                    TotalCost = 0,
                    Email = email
                };
                await basketsCollection.InsertOneAsync(newBasket);
                userBasket = newBasket;
               
            }

            var dbProduct = DbProduct.ToEntity(product);

            var productsSubCollections = userBasket.Products;
            productsSubCollections.Add(dbProduct);
            var totalCost = productsSubCollections.Sum(item => item.Quantity * item.Price);

            var result = await basketsCollection.UpdateOneAsync(
                filter: b => b.UserId == userId,
                update: Builders<DbBasket>.Update.Combine(
                    Builders<DbBasket>.Update.Set(b => b.Products, productsSubCollections),
                    Builders<DbBasket>.Update.Set(b => b.LastEdited, DateTime.Now),
                    Builders<DbBasket>.Update.Set(b => b.TotalCost, totalCost)
                ));
            Console.WriteLine($"Modified {result.ModifiedCount} lines!");
        }


        public UserBasket FindBasketById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserBasket?> FindBasketByUserIdAsync(string id)
        {
            var qBasket = await basketsCollection.FindAsync(b => b.UserId == id);
            var basket = await qBasket.SingleOrDefaultAsync();

            return basket?.ToModel();
            
        }

        public async Task<List<UserBasket>> getAllBasketsAsync()
        {
            var baskets = (await basketsCollection.FindAsync(_ => true)).ToList();

            return baskets
                .Select(b => b.ToModel())
                .ToList();
        }

        //returns the number of edits in the database
        public async Task<long> DeleteProductFromBasketAsync(string userId, string productId) {
            var qBasket = await basketsCollection.FindAsync(b => b.UserId == userId);
            var basket = await qBasket.SingleOrDefaultAsync();

            if (basket == null)
                return 0;
            var productGuid = Guid.Parse(productId);
            var updatedProductsSubcollection = basket.Products.Where(product => product.BasketSubId != productGuid).ToList();
            var totalCost = updatedProductsSubcollection.Sum(item => item.Quantity * item.Price);
                 
            var result = await basketsCollection.UpdateOneAsync(
                filter: b => b.UserId == userId ,
                update: Builders<DbBasket>.Update.Combine(
                     Builders<DbBasket>.Update.Set(b => b.Products, updatedProductsSubcollection),
                     Builders<DbBasket>.Update.Set(b => b.LastEdited, DateTime.Now),
                     Builders<DbBasket>.Update.Set(b => b.TotalCost, totalCost)
                    )
                );

            return result.ModifiedCount;
        }

        public async Task ClearBasket(string userId)
        {
            var qBasket = await basketsCollection.FindAsync(b => b.UserId == userId);
            var basket = await qBasket.SingleOrDefaultAsync();

            await basketsCollection.DeleteOneAsync(filter: b => b.UserId == userId);
        }
    }
}
