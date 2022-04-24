using BasketService.DAL;
using BasketService.Model;
using MassTransit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace BasketService.Services
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly IBasketRepository repository;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public RabbitMQService(IBasketRepository repository, ISendEndpointProvider sendEndpointProvider)
        {
            this.repository = repository;
            this._sendEndpointProvider = sendEndpointProvider;

        }

        public class OrderSubmittedEventConsumer : IConsumer<IProductWithUserId>
        {
            private readonly IBasketRepository repository;

            public OrderSubmittedEventConsumer(IBasketRepository repository) {
                this.repository = repository;
            }
            public async Task Consume(ConsumeContext<IProductWithUserId> context)
            {
                Console.WriteLine("Product recieved" + context.Message.Product);
                repository.AddProductToBasket(context.Message.Product, context.Message.UserId);
            }

        }

        public async Task ConvertBasketToOrderAsync(UserBasket basket) {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:BasketToOrderQueue"));
            Console.WriteLine("sending basket:" + basket.ToString());
            await endpoint.Send<IBasketTransfer>(new
            {
                UserBasket = basket
            });
        }

    }

    public interface IProductWithUserId
    {
        Product Product { get; set; }
        int UserId { get; set; }
    }

    public interface IBasketTransfer { 
        UserBasket Basket { get; set; }
    }

}
