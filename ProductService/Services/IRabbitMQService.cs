using ProductService.Model;

namespace ProductService.Services
{
    public interface IRabbitMQService
    {
        public Task AddProductToBasket(Product product, string userId, string email);
    }
}
