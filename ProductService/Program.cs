using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProductService.DAL;
using ProductService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");

builder.Services.AddDbContext<ProductService.DAL.EfDbContext.ProductDbContext>(options => {
    options.UseSqlServer(connectionString,
    sqlServerOptionsAction: sqlOptions =>
    {
       sqlOptions.EnableRetryOnFailure(
       maxRetryCount: 255,
       maxRetryDelay: TimeSpan.FromSeconds(1),
       errorNumbersToAdd: null);
    }
    );
});
builder.Services.AddScoped<ProductService.DAL.IProductsRepository, ProductService.DAL.ProductsRepository>();
//builder.Services.AddScoped<ProductService.DAL.EfDbContext.ProductDbContext>();

//initiate RabbitMQ for DI
builder.Services.AddMassTransit(options => {
    options.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("RabbitMQ", "/", h =>
        {
            h.Password("guest");
            h.Username("guest");
        });
        
    });
});

builder.Services.AddScoped<IRabbitMQService, RabbitMQService>();

//var options = new DbContextOptionsBuilder<ProductService.DAL.EfDbContext.ProductDbContext>().UseSqlServer(connectionString).Options;
//using (var db = new ProductService.DAL.EfDbContext.ProductDbContext(options)) {
//    db.Database.EnsureCreated();
//}

builder.Services.AddSwaggerGen();
var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<ProductService.DAL.EfDbContext.ProductDbContext>();
    context.Database.EnsureCreated();
}

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
