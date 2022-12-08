using AutoMapper;
using BasketService.Model;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Models;
using PaymentService.Models.InitPayment;
using PaymentService.services;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PaymentService.Controllers;

[ApiController]
[Route("api/payment")]
public class PaymentController : ControllerBase
{

  private readonly ILogger<PaymentController> _logger;
  private readonly IHttpClientFactory factory;
  private readonly IConfiguration confirguration;
  private HttpClient tokenHttpClient;
  private readonly IMapper mapper;
  private readonly IRabbitMqService rabbitMQ;
  public PaymentController(ILogger<PaymentController> logger, IHttpClientFactory clientFactory, IConfiguration configuration, IMapper mapper, IRabbitMqService rabbitMQ)
  {
    this.mapper = mapper;
    this.confirguration = configuration;
    this.rabbitMQ = rabbitMQ;
    factory = clientFactory;
    _logger = logger;
    InitTokenClient();
  }

  [HttpPost("create")]
  public async Task<ActionResult<CreateOrder>> CreateOrder([FromBody] UserBasket basket)
  {
    Console.WriteLine("CreateOrder");
    Console.WriteLine(basket);
    var accessToken = await AquirePayPalAccessToken();
    if (accessToken == null || string.IsNullOrEmpty(accessToken.Access_token))
      return Unauthorized("Faild to authenticate user");
    var apiHttpClinet = CreateHttpClient(accessToken);
    //FileStream stream = System.IO.File.OpenRead(@"./Models/mockPayment.json");
    //var file = await JsonSerializer.DeserializeAsync<Order>(stream);
    //Console.WriteLine("File:" + stream.ToString());
    var newOrder = new InitOrder()
    {
      Intent = "CAPTURE",
      PurchaseUnits = new List<Models.InitPayment.PurchaseUnit>() {
        new PaymentService.Models.InitPayment.PurchaseUnit() {
         Amount = new Models.InitPayment.Amount() {
          CurrencyCode = "HUF",
          Value = basket.TotalCost.ToString()
         },
         Payee = new Models.InitPayment.Payee(){
          EmailAdress = "sb-dnoc4320943156@business.example.com",
          MerchantId = "CMRFZV8SL7DEN"
         }
        }
      }
    };
    var newOrderSerialized = JsonSerializer.Serialize(newOrder);
    Console.WriteLine(newOrderSerialized);
    var response = await apiHttpClinet.PostAsJsonAsync("", newOrderSerialized);
    var data = await response.Content.ReadAsStringAsync();
    Console.WriteLine("Res:" + data.ToString());
    var order = JsonSerializer.Deserialize<CreateOrder>(data);
    return Ok(order);
  }

  [HttpPost(Name = "CaptureOrder")]
  public async Task<ActionResult> CaptureOrder([FromBody] CaptureOrder approve) {
    var accessToken = await AquirePayPalAccessToken();
    if (accessToken == null || string.IsNullOrEmpty(accessToken.Access_token))
      return Unauthorized("Faild to authenticate user");
    var apiHttpClinet = CreateHttpClient(accessToken);
    var response1 = await apiHttpClinet.GetAsync($"/{approve.id}");
    var data1 = await response1.Content.ReadAsStringAsync();
    Console.WriteLine("Res1 statusz:" + data1.ToString());

    var response = await apiHttpClinet.PostAsJsonAsync($"/{approve.id}/capture", "");
    var data = await response.Content.ReadAsStringAsync();
    Console.WriteLine("Res:" + data.ToString());
    Console.WriteLine(approve);
    return Ok();
  }

  [HttpPost("complete")]
  public async Task<ActionResult> CompleteBasket([FromBody] UserBasket basket) {
    var name = decodeUserData("UserName");
    await this.rabbitMQ.BasketTransactionCompleted(basket, name);
    return Ok();
  }


    private async Task<PayPalTokenResponse?> AquirePayPalAccessToken() {
    var tokenResponse = await tokenHttpClient.PostAsync("", new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            })
    );

    return await tokenResponse.Content.ReadFromJsonAsync<PayPalTokenResponse>(); 
  }

  private HttpClient CreateHttpClient(PayPalTokenResponse token) {
    var apiHttpClinet = factory.CreateClient("ApiHttpClient");
    apiHttpClinet.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Access_token);
    apiHttpClinet.DefaultRequestHeaders.Add("PayPal-Request-Id", Guid.NewGuid().ToString());
    apiHttpClinet.BaseAddress = new Uri(confirguration.GetRequiredSection("PayPal").GetValue<string>("PayPalOrderApi"));
    return apiHttpClinet;
  }

  private void InitTokenClient() {
    tokenHttpClient = factory.CreateClient("PayPal");
    tokenHttpClient.BaseAddress = new Uri(confirguration.GetRequiredSection("PayPal").GetValue<string>("PayPalTokenApi"));
    var clientId = confirguration.GetRequiredSection("PayPal").GetValue<string>("ClientId");
    var clientSecret = confirguration.GetRequiredSection("PayPal").GetValue<string>("ClientSecret");
    var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{clientId}:{clientSecret}"));
    tokenHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
  }

  private string decodeUserData(string data)
  {
    var encodedUserData = Request.Headers[data].ToString() ?? "";
    return Encoding.UTF8.GetString(Convert.FromBase64String(encodedUserData));
  }

}
