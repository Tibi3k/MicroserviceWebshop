﻿using BasketService.Services;
using MassTransit;
using ProductService.Model;
using RabbitMQ.Client;
using System.Text.Json;

namespace ProductService.Services
{
    public class RabbitMQService : IRabbitMQService
    { 
        private readonly ISendEndpointProvider _sendEndpointProvider;
        public RabbitMQService(ISendEndpointProvider sendEndpointProvider)
        {
            this._sendEndpointProvider = sendEndpointProvider;
        }

        public async Task AddProductToBasket(Product product, string userId, string email) {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:ProductToBasketQueue3"));
            Console.WriteLine("sending product:" + product);
            await endpoint.Send<IProductWithUserId>(new
            {
                Product = product,
                UserId = userId,
                Email = email,
                AddTime = DateTime.Now
            });
        }
    }
}

namespace BasketService.Services
{
    public interface IProductWithUserId
    {
        Product Product { get; set; }
        string UserId { get; set; }
        string Email { get; set; }
        DateTime AddTime { get; set; }
    }
}
