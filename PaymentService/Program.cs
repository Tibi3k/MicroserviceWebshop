using MassTransit;
using PaymentService.Models;
using PaymentService.services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IRabbitMqService, RabbitMqService>();
builder.Services.AddMassTransit(options => {
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
  });
});
builder.Services.AddAutoMapper(config =>
{
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
