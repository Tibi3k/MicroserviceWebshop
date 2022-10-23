using BasketService.Model;
using BasketService.Services;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using OrderService.DAL.DbModel;

namespace PaymentService.services
{
  public class RabbitMqService : IRabbitMqService
  {
    private readonly ISendEndpointProvider _sendEndpointProvider;
    public RabbitMqService(ISendEndpointProvider sendEndpointProvider)
    {
      this._sendEndpointProvider = sendEndpointProvider;
    }
    public async Task<bool> BasketTransactionCompleted(UserBasket basket, string name)
    {
      try
      {
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:PaymentBasketQueue"));
        await endpoint.Send<IBasketTransfer>(new
        {
          Email = basket.Email,
          TotalCost = basket.TotalCost,
          UserId = basket.UserId,
          OrderTime = DateTime.Now,
          Name = name,
          OrderId = "",
          Products = new List<Product>()
        });
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
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