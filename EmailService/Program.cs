using EmailService;
using EmailService.RabbitMQ;
using MassTransit;
using static EmailService.RabbitMQ.MailService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        services.AddTransient<MailService>(context =>
        {
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
                cfg.UseDelayedRedelivery(r => r.Intervals(
                    TimeSpan.FromMinutes(1), 
                    TimeSpan.FromMinutes(2), 
                    TimeSpan.FromMinutes(3),
                    TimeSpan.FromMinutes(10),
                    TimeSpan.FromMinutes(20)
                    ));

                cfg.ReceiveEndpoint("OrderConfirmationEmailQueue", e =>
                {
                    var service = context.GetRequiredService(typeof(MailService)) as MailService;
                    e.Consumer(() => new OrderSubmittedEventConsumer(service));
                });
            });
        });

        services.AddOptions<MassTransitHostOptions>()
            .Configure(options =>
            {
                
                options.WaitUntilStarted = true;
                //options.StartTimeout = TimeSpan.FromSeconds(30);
            });
    })
    .Build();

await host.RunAsync();
