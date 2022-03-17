﻿using BasketService.DAL;
using BasketService.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace BasketService.Services
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly EventingBasicConsumer consumer;
        private readonly IBasketRepository repository;

        public RabbitMQService(string connectionString)
        {
            var connectionFactory = new ConnectionFactory();
            connectionFactory.Uri = new Uri(connectionString);
            //connectionFactory.ClientProvidedName = "app:ProductService component:event-consumer";
            this.connection = connectionFactory.CreateConnection();
            this.channel = this.connection.CreateModel();
            channel.QueueDeclare(queue: "ProductToBasketQueue",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            this.consumer = new EventingBasicConsumer(channel);
            ActivateEventConsumer();
            channel.BasicConsume(queue: "ProductToBasketQueue", false, this.consumer);
            
        }

        private void ActivateEventConsumer() {
            consumer.Received +=  (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var serializedObject = System.Text.Encoding.Default.GetString(body);
                try
                {
                    Product product = JsonSerializer.Deserialize<Product>(serializedObject, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (product == null)
                        Console.WriteLine("null");
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("EX:",ex?.InnerException?.Message, ex?.InnerException?.StackTrace, ex.Message, ex.StackTrace);
                }

            };
        }

    }
}
