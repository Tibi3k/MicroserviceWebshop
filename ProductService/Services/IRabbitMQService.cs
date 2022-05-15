using ProductService.Model;

namespace ProductService.Services
{
    public interface IRabbitMQService
    {
        public Task<bool> AddProductToBasket(Product product, string userId, string email);
    }
}
