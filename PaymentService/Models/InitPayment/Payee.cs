using System.Text.Json.Serialization;

namespace PaymentService.Models.InitPayment;

public class Payee
{
  [JsonPropertyName("email_address")]
  public string EmailAdress { get; set; }
  [JsonPropertyName("merchant_id")]
  public string MerchantId { get; set; } 
}
