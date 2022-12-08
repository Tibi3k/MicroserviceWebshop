using System.Text.Json.Serialization;

namespace PaymentService.Models.InitPayment;

public class Amount
{
  [JsonPropertyName("currency_code")]
  public string CurrencyCode { get; set; } = "HUF";
  [JsonPropertyName("value")]
  public string Value { get; set; }
}
