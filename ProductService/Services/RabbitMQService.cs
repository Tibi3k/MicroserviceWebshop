using BasketService.Services;
using MassTransit;
using ProductService.Model;
using RabbitMQ.Client;
using System.Text.Json;

namespace ProductService.Services
{
    public class RabbitMQService : IRabbitMQService
    { 
        private readonly ISendEndpointProvider _sendEndpointProvider;
        public RabbitMQService(ISendEndpointProvider sendEndpointProvider) {
            this._sendEndpointProvider = sendEndpointProvider;
        }

        public void Send()
        {

        }



        public async Task AddProductToBasket(Product product, int userId) {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:ProductToBasketQueue3"));
            Console.WriteLine("sending product:" + product);
            await endpoint.Send<IProductWithUserId>(new
            {
                Product = product,
                UserId = userId
            });
        }
    }
}

namespace BasketService.Services
{
    public interface IProductWithUserId
    {
        Product Product { get; set; }
        int UserId { get; set; }
    }
}
