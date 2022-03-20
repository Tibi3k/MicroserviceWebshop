using ProductService.Model;
using RabbitMQ.Client;
using System.Text.Json;

namespace ProductService.Services
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly IConnection connection;
        private readonly IModel channel;

        public RabbitMQService(string connectionString) {
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
        }

        public void Send()
        {
            byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes("Hello, world!");
            channel.BasicPublish("", "ProductToBasketQueue", null, messageBodyBytes);
        }

        public void AddProductToBasket(Product product, int userId) {
            string jsonObject = JsonSerializer.Serialize(product);
            jsonObject += "userId:" + userId;
            byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes(jsonObject);
            channel.BasicPublish("", "ProductToBasketQueue", null, messageBodyBytes);
        }
    }
}
