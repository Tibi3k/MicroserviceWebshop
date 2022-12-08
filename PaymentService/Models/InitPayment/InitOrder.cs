using System.Text.Json.Serialization;

namespace PaymentService.Models.InitPayment;

public class InitOrder
{
  [JsonPropertyName("intent")]
  public string Intent { get; set; } = "CAPTURE";
  [JsonPropertyName("purchase_units")]
  public List<PurchaseUnit> PurchaseUnits { get; set; } = new List<PurchaseUnit>();
}
