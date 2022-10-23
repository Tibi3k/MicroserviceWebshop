using BasketService.DAL;
using BasketService.Model;
using MassTransit;
using Microsoft.Graph;
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
            private readonly IServiceProvider serviceProvider;

            public OrderSubmittedEventConsumer(IServiceProvider serviceProvider) {
                this.serviceProvider = serviceProvider;
            }
            public async Task Consume(ConsumeContext<IProductWithUserId> context)
            {
                Console.WriteLine("Product recieved" + context.Message.Product);
                var scope = serviceProvider.CreateScope();
                var repository = scope.ServiceProvider.GetService<IBasketRepository>();
                var product = new Product
                {
                    Id = context.Message.Product.Id,
                    Name = context.Message.Product.Name,
                    Category = context.Message.Product.Category,
                    Description = context.Message.Product.Description,
                    Quantity = context.Message.Product.Quantity,
                    Price = context.Message.Product.Price,
                    BasketSubId = Guid.NewGuid().ToString(),
                    AddTime = context.Message.AddTime
                };
                await repository.AddProductToBasketAsync(product, context.Message.UserId, context.Message.Email);
            }

        }
    public class PaymentCompletedEventConsumer : IConsumer<IBasketTransfer>
    {
      private readonly IServiceProvider serviceProvider;

      public PaymentCompletedEventConsumer(IServiceProvider serviceProvider)
      {
        this.serviceProvider = serviceProvider;
      }
      public async Task Consume(ConsumeContext<IBasketTransfer> context)
      {
        var scope = serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetService<IBasketRepository>();
        var rabbitMq = scope.ServiceProvider.GetService<IRabbitMQService>();
        var basket = await repository.FindBasketByUserIdAsync(context.Message.UserId);
        var result1 = await rabbitMq.ConvertBasketToOrderAsync(basket, context.Message.Name);
        var result2 = await rabbitMq.SendOrderConfirmationEmailAsync(basket, context.Message.Name);
        await repository.ClearBasket(context.Message.UserId);
      }
    }

    public async Task<bool> ConvertBasketToOrderAsync(UserBasket basket, string name) {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:BasketToOrderQueue"));
            Console.WriteLine("sending basket:" + basket.ToString());
            try
            {
                await endpoint.Send<IBasketTransfer>(new
                {
                    Email = basket.Email,
                    OrderId = basket.Id,
                    Products = basket.Products,
                    Name = name,
                    UserId = basket.UserId,
                    TotalCost = basket.TotalCost,
                    OrderTime = DateTime.Now
                }).WaitAsync(TimeSpan.FromSeconds(10));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<bool> SendOrderConfirmationEmailAsync(UserBasket basket, string name)
        {
            try { 
                var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:OrderConfirmationEmailQueue"));
                Console.WriteLine("sending basket:" + basket.Products.ToList()[0].ToString() + ", " + basket.Id );
                await endpoint.Send<IBasketTransfer>(new
                {
                    Email = basket.Email,
                    OrderId = basket.Id,
                    Products = basket.Products,
                    Name = name
                });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> ReturnAvailableAmountToProduct(int productId, int quantity)
        {
            try { 
                var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:ProductQuantityReturnQueue"));
                await endpoint.Send<IQuantityTransfer>(new
                {
                    productId = productId,
                    quantity = quantity
                });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }

    public interface IProductWithUserId
    {
        RecievedProduct Product { get; set; }
        string UserId { get; set; }
        string Email { get; set; }
        DateTime AddTime { get; set; }
    }

    public class RecievedProduct
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = string.Empty;
        public int Price { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }

    }

    public interface IBasketTransfer { 
        string Email { get; set; }
        string OrderId { get; set; }
        List<Product> Products { get; set; }
        string Name { get; set; }
        int TotalCost { get; set; }
        string UserId { get; set; }
        DateTime OrderTime { get; set; }
    }

    public interface IQuantityTransfer { 
        int productId { get; set; }
        int quantity { get; set; }
    }

}
