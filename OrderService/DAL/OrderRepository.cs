﻿using MongoDB.Driver;

namespace OrderService.DAL.DbModel;

public class OrderRepository : IOrderRepository
{
    private readonly IMongoCollection<DbOrder> orderCollection;
    public OrderRepository(IMongoDatabase database)
    {
        this.orderCollection = database.GetCollection<DbOrder>("orders");
    }

    public async Task AddNewOrder(DbOrderItem orderItem, string Email, string Name, string UserId) {
        var order = await this.orderCollection
            .Find(o => o.UserId == UserId)
            .SingleOrDefaultAsync();
        if (order == null) {
            order = new DbOrder
            {
                Email = Email,
                UserId = UserId,
                Username = Name
            };

            await orderCollection.InsertOneAsync(order);
        }
        order.OrderItems.Add(orderItem);


        var result = await orderCollection.UpdateOneAsync(
            filter: b => b.UserId == UserId,
            update: Builders<DbOrder>.Update.Set(b => b.OrderItems, order.OrderItems)
        );
        Console.WriteLine($"Modified {result.ModifiedCount} lines!");
    }
}
