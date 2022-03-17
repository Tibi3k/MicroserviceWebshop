using ProductService.Model;

namespace ProductService.Services
{
    public interface IRabbitMQService
    {
        public void Send();
        public void AddProductToBasket(Product product);
    }
}
