using BasketService.Model;

namespace BasketService.DAL
{
    public interface IBasketRepository
    {
        Task<List<UserBasket>> getAllBasketsAsync();
        UserBasket FindBasketById(string id);
        Task<UserBasket?> FindBasketByUserIdAsync(int id);
        Task AddProductToBasketAsync(Product product, int userId);

        Task<long> DeleteProductFromBasketAsync(int userId, int productId);

        void Test();
    }
}
