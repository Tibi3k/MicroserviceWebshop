namespace OrderService.DAL.DbModel;

public interface IOrderRepository
{
    Task AddNewOrder(DbOrderItem orderItem, string Email, string Name, string UserId);
}
