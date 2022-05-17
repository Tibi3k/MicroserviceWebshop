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
        cfg.ReceiveEndpoint("ProductQuantityReturnQueue", e =>
        {
            var service = context.GetRequiredService(typeof(IServiceProvider)) as IServiceProvider;
            e.Consumer(() => new ProductQuantityRestoreConsumer(service));
        });
    });
});

//make sure that the message bus is ready
builder.Services.AddOptions<MassTransitHostOptions>()
    .Configure(options =>
    {
        //options.WaitUntilStarted = true;
    });

builder.Services.AddCors(o => o.AddPolicy("default", builder =>
{
    builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
}));

builder.Services.AddScoped<IRabbitMQService, RabbitMQService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("User", policy => 
        policy.RequireClaim("http://schemas.microsoft.com/identity/claims/objectidentifier"));
    options.AddPolicy("Admin", policy =>
        policy.RequireClaim("jobTitle", "Admin"));
});

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


app.UseAuthentication();
// End of the block you add

app.UseAuthorization();


app.MapControllers();

app.Run();
