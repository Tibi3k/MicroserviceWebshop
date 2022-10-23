using BasketService.Model;

namespace PaymentService.services;

public interface IRabbitMqService
{
  public Task<bool> BasketTransactionCompleted(UserBasket basket, string name);

}
