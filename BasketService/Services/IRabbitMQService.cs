using BasketService.Model;

namespace BasketService.Services
{
    public interface IRabbitMQService
    {
        public Task ConvertBasketToOrderAsync(UserBasket basket, string name);
        public Task SendOrderConfirmationEmailAsync(UserBasket basket, string name);
        public Task ReturnAvailableAmountToProduct(int productId, int quantity);
    }
}
