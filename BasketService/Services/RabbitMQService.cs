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

        public RabbitMQService(IBasketRepository repository)
        {
            this.repository = repository;
            
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

    }

    public interface IProductWithUserId
    {
        Product Product { get; set; }
        int UserId { get; set; }
    }
}
