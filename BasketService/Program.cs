using BasketService.DAL;
using BasketService.Services;
using MassTransit;
using MongoDB.Driver;
using static BasketService.Services.RabbitMQService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRabbitMQService, RabbitMQService>();


var mongoDBConnectionString = builder.Configuration.GetConnectionString("MongoDBConnection");
builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoDBConnectionString));

builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase("basket");
});
var service = builder.Services.AddScoped<IBasketRepository, BasketRepository>();

//initiate Mass Transit RabbitMQ for DI
builder.Services.AddMassTransit(options => {
    options.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("RabbitMQ", "/", h =>
        {
            h.Password("guest");
            h.Username("guest");
        });

        cfg.ReceiveEndpoint("ProductToBasketQueue3", e =>
        {
            var service = context.GetRequiredService(typeof(IBasketRepository)) as IBasketRepository;
            e.Consumer(() => new OrderSubmittedEventConsumer(service));
            e.RethrowFaultedMessages();
        });
    });
});

builder.Services.AddOptions<MassTransitHostOptions>()
    .Configure(options =>
    {
        options.WaitUntilStarted = true;
    });

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


