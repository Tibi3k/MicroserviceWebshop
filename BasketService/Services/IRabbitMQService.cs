using BasketService.Model;

namespace BasketService.Services
{
    public interface IRabbitMQService
    {
        public Task<bool> ConvertBasketToOrderAsync(UserBasket basket, string name);
        public Task<bool> SendOrderConfirmationEmailAsync(UserBasket basket, string name);
        public Task<bool> ReturnAvailableAmountToProduct(int productId, int quantity);
    }
}
