using ProductService.Model;

namespace ProductService.Services
{
    public interface IRabbitMQService
    {
        public void Send();
        public Task AddProductToBasket(Product product, int userId);
    }
}
