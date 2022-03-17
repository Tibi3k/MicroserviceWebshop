using BasketService.Model;

namespace BasketService.DAL
{
    public class BasketRepository : IBasketRepository
    {
        public void AddProductToBasket(Product product)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
