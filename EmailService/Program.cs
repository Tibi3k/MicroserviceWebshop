using EmailService;
using EmailService.RabbitMQ;
using MassTransit;
using static EmailService.RabbitMQ.MailService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        services.AddTransient<MailService>(context => {
            var email = configuration.GetSection("Email").GetValue<string>("EmailAddress");
            var displayName = configuration.GetSection("Email").GetValue<string>("DisplayName");
            var password = configuration.GetSection("Email").GetValue<string>("Password");
            return new MailService(email, displayName, password);
        });

        services.AddMassTransit(options => {
            options.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("RabbitMQ", "/", h =>
                {
                    h.Password("guest");
                    h.Username("guest");
                });
                cfg.ReceiveEndpoint("OrderConfirmationEmailQueue", e =>
                {
                    var service = context.GetRequiredService(typeof(MailService)) as MailService;
                    e.Consumer(() => new OrderSubmittedEventConsumer(service));
                    e.RethrowFaultedMessages();
                });
            });
        });
    })
    .Build();

await host.RunAsync();
