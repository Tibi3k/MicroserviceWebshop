using System.Text.Json.Serialization;

namespace PaymentService.Models;

public class CreateOrder
{
  public string id { get; set; }
  public string status { get; set; }
  public List<Link> links { get; set; }
}


