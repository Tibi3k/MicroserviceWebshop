using BasketService.Model;

namespace BasketService.Services
{
    public interface IRabbitMQService
    {
        public Task ConvertBasketToOrderAsync(UserBasket basket);
        public Task SendOrderConfirmationEmailAsync(UserBasket basket);
    }
}
