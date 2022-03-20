using BasketService.DAL;
using BasketService.Services;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



//initiate MongoDB connection
var mongoDBConnectionString = builder.Configuration.GetConnectionString("MongoDBConnection");
var mongo = new MongoClient(mongoDBConnectionString);
builder.Services.AddSingleton<IMongoClient>(mongo);
var db = mongo.GetDatabase("basket");
builder.Services.AddSingleton(db);
var repo = new BasketRepository(db);
var service = builder.Services.AddSingleton<IBasketRepository>(repo);

//initiate RabbitMQ for DI
var rabbitMQConntectionString = builder.Configuration.GetConnectionString("RabbitMQConnection");
var rabbitMQ = new BasketService.Services.RabbitMQService(rabbitMQConntectionString, repo);
builder.Services.AddSingleton<BasketService.Services.IRabbitMQService>(rabbitMQ);



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


