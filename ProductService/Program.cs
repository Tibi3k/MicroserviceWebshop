using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProductService.DAL;
using ProductService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Hellang.Middleware.ProblemDetails;
using static ProductService.Services.RabbitMQService;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.CaptureStartupErrors(true);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");


builder.Services.AddDbContext<ProductService.DAL.EfDbContext.ProductDbContext>(options => {
    options.UseSqlServer(connectionString);
});
builder.Services.AddScoped<IProductsRepository,ProductsRepository>();

builder.Services.AddProblemDetails(options =>
{
    options.IncludeExceptionDetails = (ctx, ex) => false;
    options.Map<Exception>(
        (ctx, ex) =>
        {
            Console.WriteLine("ex:" + ex.ToString());
            global::System.Console.WriteLine("type:" + ex.GetType());
            var pd = StatusCodeProblemDetails.Create(StatusCodes.Status500InternalServerError);
            pd.Title = "Something went wrong, please try again later!";
            return pd;
        });
});

//initiate RabbitMQ for DI
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

        cfg.ReceiveEndpoint("ProductQuantityReturnQueue", e =>
        {
            var service = context.GetRequiredService(typeof(IServiceProvider)) as IServiceProvider;
            e.Consumer(() => new ProductQuantityRestoreConsumer(service));
        });
    });
});

builder.Services.AddCors(o => o.AddPolicy("default", builder =>
{
    builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
}));

builder.Services.AddScoped<IRabbitMQService, RabbitMQService>();

builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("default");
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
