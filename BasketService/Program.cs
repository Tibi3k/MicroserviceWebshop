using BasketService.DAL;
using BasketService.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
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

builder.Services.AddScoped(sp =>
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
            var service = context.GetRequiredService(typeof(IServiceProvider)) as IServiceProvider;
            e.Consumer(() => new OrderSubmittedEventConsumer(service));
            e.RethrowFaultedMessages();
        });
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("User", policy =>
        policy.RequireClaim("http://schemas.microsoft.com/identity/claims/objectidentifier"));
    options.AddPolicy("Admin", policy =>
        policy.RequireClaim("jobTitle", "Admin"));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


