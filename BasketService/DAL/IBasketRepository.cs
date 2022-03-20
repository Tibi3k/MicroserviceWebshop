using BasketService.Model;

namespace BasketService.DAL
{
    public interface IBasketRepository
    {
        List<UserBasket> getAllBaskets();
        UserBasket FindBasketById(string id);
        UserBasket FindBasketByUserId(int id);
        void AddProductToBasket(Product product, int userId);
    }
}
