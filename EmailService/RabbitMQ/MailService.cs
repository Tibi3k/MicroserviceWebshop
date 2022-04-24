using BasketService.Services;
using EmailService.RabbitMQ;
using MassTransit;
using MimeKit;
using System;
using System.Collections.Generic;
using MailKit.Net.Smtp;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService.RabbitMQ
{
    public class MailService
    {
        private readonly string EmailAddress;
        private readonly string DisplayName;
        private readonly string Password;

        public MailService(string EmailAddress, string DisplayName, string Password)
        {
            this.EmailAddress = EmailAddress;
            this.DisplayName = DisplayName;
            this.Password = Password;
        }
        public class OrderSubmittedEventConsumer : IConsumer<IBasketTransfer>
        {
            private readonly MailService mailService;
            public OrderSubmittedEventConsumer(MailService mailService)
            {
                this.mailService = mailService;
            }
            public async Task Consume(ConsumeContext<IBasketTransfer> context)
            {
                mailService.SendEmail(
                    context.Message.Products, 
                    context.Message.OrderId, 
                    context.Message.Name, 
                    context.Message.Email);
            }
        }


        public void SendEmail(List<Product> products, string OrderId, string name, string email) {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(DisplayName, EmailAddress));
            message.To.Add(new MailboxAddress(name, email));
            message.Subject = "Order confirmation";
            message.Body = new TextPart("plain")
            {
                Text = CreateEmailFromTemplate(products, OrderId, name)
            };

            using (var client = new SmtpClient()) {
                client.Connect("smtp.gmail.com", 587);
                client.Authenticate(EmailAddress, Password);

                client.Send(message);
                client.Disconnect(true);
            }
        }

        private string CreateEmailFromTemplate(List<Product> products,string OrderId, string name) {
            var productsString = String.Join(
                "\n", products.Select(product => 
                    $"- {product.Quantity}x {product.Name}: {product.Price * product.Quantity}ft")
                );
            productsString += $"\n\nTotal: {products.Sum(b => b.Price * b.Quantity)}ft";

            return $"Dear {name},\n\nThank you for ordering from us.\n\nYour order is ready.\n\nYour order id is: {OrderId}\n\nYou can review your order status at any time by visiting Your Account.\n\nYour order contains:\n{productsString}\n\nWe hope you enjoyed your shopping experience with us and that you will visit us again soon.";
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
    }
}

