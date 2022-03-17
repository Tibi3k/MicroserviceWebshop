using Microsoft.EntityFrameworkCore;
using ProductService.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");

builder.Services.AddDbContext<ProductService.DAL.EfDbContext.ProductDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<ProductService.DAL.IProductsRepository, ProductService.DAL.ProductsRepository>();
builder.Services.AddScoped<ProductService.DAL.EfDbContext.ProductDbContext>();

//initiate RabbitMQ for DI
var rabbitMQConntectionString = builder.Configuration.GetConnectionString("RabbitMQConnection");
var rabbitMQ = new ProductService.Services.RabbitMQService(rabbitMQConntectionString);
builder.Services.AddSingleton<ProductService.Services.IRabbitMQService>(rabbitMQ);

var options = new DbContextOptionsBuilder<ProductService.DAL.EfDbContext.ProductDbContext>().UseSqlServer(connectionString).Options;
using (var db = new ProductService.DAL.EfDbContext.ProductDbContext(options)) {
    db.Database.EnsureCreated();
}

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
