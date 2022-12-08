using System.Text.Json.Serialization;

namespace PaymentService.Models.InitPayment;

public class PurchaseUnit
{
  [JsonPropertyName("amount")]
  public Amount Amount { get; set; }
  [JsonPropertyName("payee")]
  public Payee Payee { get; set; }
}
