using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProductService.DAL;
using ProductService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");

builder.Services.AddDbContext<ProductService.DAL.EfDbContext.ProductDbContext>(options => {
    options.UseSqlServer(connectionString
    //,
    //sqlServerOptionsAction: sqlOptions =>
    //{
    //   sqlOptions.EnableRetryOnFailure(
    //   maxRetryCount: 255,
    //   maxRetryDelay: TimeSpan.FromSeconds(1),
    //   errorNumbersToAdd: null);
    //}
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

//make sure that the message bus is ready
builder.Services.AddOptions<MassTransitHostOptions>()
    .Configure(options =>
    {
        options.WaitUntilStarted = true;
        options.StartTimeout = TimeSpan.FromSeconds(30);
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
app.UseCors("default");
app.UseHttpsRedirection();

app.UseAuthentication();
// End of the block you add

app.UseAuthorization();


app.MapControllers();

app.Run();
