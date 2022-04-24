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
        public async Task AddProductToBasketAsync(Product product, int userId)
        {

            var qBasket = await basketsCollection.FindAsync(b => b.UserId == userId);
            var userBasket = await qBasket.FirstOrDefaultAsync();

            if (userBasket == null) {
                var newBasket = new DbBasket()
                {
                    UserId = userId,
                    LastEdited = DateTime.Now,
                    Products = new List<DbProduct>(),
                    TotalCost = 0
                };
                await basketsCollection.InsertOneAsync(newBasket);
                userBasket = newBasket;
               
            }

            var dbProduct = DbProduct.ToEntity(product);

            var productsSubCollections = userBasket.Products;
            productsSubCollections.Add(dbProduct);

            var result = await basketsCollection.UpdateOneAsync(
                filter: b => b.UserId == userId,
                update: Builders<DbBasket>.Update.Set(b => b.Products, productsSubCollections)
                );
            Console.WriteLine($"Modified {result} lines!");
        }


        public UserBasket FindBasketById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserBasket?> FindBasketByUserIdAsync(int id)
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
        public async Task<long> DeleteProductFromBasketAsync(int userId, int productId) {
            var qBasket = await basketsCollection.FindAsync(b => b.UserId == userId);
            var basket = await qBasket.SingleOrDefaultAsync();

            if (basket == null)
                return 0;

            var updatedProductsSubcollection = basket.Products.ToList();
            updatedProductsSubcollection.Where(product => product.Id != productId);

            var result = await basketsCollection.UpdateOneAsync(
                filter: b => b.UserId == userId,
                update: Builders<DbBasket>.Update.Set(b => b.Products, updatedProductsSubcollection)
                );

            return result.ModifiedCount;
        }


        public void Test() {
            Console.WriteLine("BasketRepostiroy test called!");
        }
    }
}
