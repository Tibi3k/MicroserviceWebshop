using Microsoft.AspNetCore.Mvc;
using PaymentService.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PaymentService.Controllers;

[ApiController]
[Route("payment")]
public class PaymentController : ControllerBase
{

  private readonly ILogger<PaymentController> _logger;
  private readonly IHttpClientFactory factory;
  private readonly IConfiguration confirguration;
  private HttpClient tokenHttpClient;
  public PaymentController(ILogger<PaymentController> logger, IHttpClientFactory clientFactory, IConfiguration configuration)
  {
    this.confirguration = configuration;
    factory = clientFactory;
    _logger = logger;
    InitHttpClient();
  }

  [HttpPost(Name = "GetWeatherForecast")]
  public async Task<ActionResult<IEnumerable<WeatherForecast>>> Post()
  {
    var accessToken = await AquirePayPalAccessToken();
    if (accessToken == null || string.IsNullOrEmpty(accessToken.Access_token))
      return Unauthorized("Faild to authenticate user");
    var apiHttpClinet = factory.CreateClient("ApiHttpClient");
    apiHttpClinet.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Access_token);
    apiHttpClinet.DefaultRequestHeaders.Add("PayPal-Request-Id", Guid.Parse("7b92603e-77ed-4896-8e78-5dea2050476a").ToString());
    apiHttpClinet.BaseAddress = new Uri(confirguration.GetRequiredSection("PayPal").GetValue<string>("PayPalOrderApi"));    
    FileStream stream = System.IO.File.OpenRead(@"D:\save\Egyetem\hatodik\Onlab\projekt\MicroserviceWebshop\PaymentService\Models\mockPayment.json");
    var file = await JsonSerializer.DeserializeAsync<Order>(stream);
    Console.WriteLine("File:" + stream.ToString());
    var response = await apiHttpClinet.PostAsJsonAsync("", file);
    var text = await response.Content.ReadAsStringAsync();
    Console.WriteLine(text);
    Console.WriteLine(accessToken?.Access_token);
    return new List<WeatherForecast>();

  }

  private async Task<PayPalTokenResponse?> AquirePayPalAccessToken() {
    var tokenResponse = await tokenHttpClient.PostAsync("", new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            })
    );

    return await tokenResponse.Content.ReadFromJsonAsync<PayPalTokenResponse>(); 
  }

  private void InitHttpClient() {
    tokenHttpClient = factory.CreateClient("PayPal");
    tokenHttpClient.BaseAddress = new Uri(confirguration.GetRequiredSection("PayPal").GetValue<string>("PayPalTokenApi"));
    var clientId = confirguration.GetRequiredSection("PayPal").GetValue<string>("ClientId");
    var clientSecret = confirguration.GetRequiredSection("PayPal").GetValue<string>("ClientSecret");
    var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{clientId}:{clientSecret}"));
    tokenHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
  }
}
