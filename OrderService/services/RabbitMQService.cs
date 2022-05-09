using BasketService.Services;
using MassTransit;
using OrderService.DAL.DbModel;

namespace OrderService.services
{

    public class OrderSubmittedEventConsumer : IConsumer<IBasketTransfer>
    {
        private readonly IOrderRepository repository;
        public OrderSubmittedEventConsumer(IOrderRepository repository)
        {
            this.repository = repository;
        }
        public async Task Consume(ConsumeContext<IBasketTransfer> context)
        {
            var orderItem = new DbOrderItem
            {
                products = context.Message.Products,
                TotalCost = context.Message.TotalCost,
                OrderDate = context.Message.OrderTime,
                OrderId = context.Message.OrderId
            };
            await repository.AddNewOrder(orderItem,context.Message.Email, context.Message.Name, context.Message.UserId);
        }
    }
}

namespace BasketService.Services
{
    public interface IBasketTransfer
    {
        string Email { get; set; }
        string OrderId { get; set; }
        List<Product> Products { get; set; }
        string Name { get; set; }
        int TotalCost { get; set; }
        string UserId { get; set; }
        DateTime OrderTime { get; set; }
    }
}