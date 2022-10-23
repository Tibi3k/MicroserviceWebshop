using System.Text.Json.Serialization;

namespace PaymentService.Models;

public class Address
{
  public string? country_code { get; set; }
  public string? address_line_1 { get; set; }
  public string? admin_area_2 { get; set; }
  public string? admin_area_1 { get; set; }
  public string? postal_code { get; set; }
}

public class Amount
{
  public string? value { get; set; }
  public string? currency_code { get; set; }
}

public class Capture
{
  public string? status { get; set; }
  public string? id { get; set; }
  public bool final_capture { get; set; }
  public DateTime create_time { get; set; }
  public DateTime update_time { get; set; }
  public Amount amount { get; set; }
  public SellerProtection seller_protection { get; set; }
  public List<Link>? links { get; set; }
}

public class Link
{
  public string? href { get; set; }
  public string? rel { get; set; }
  public string? method { get; set; }
  public string? title { get; set; }
}

public class Name
{
  public string? given_name { get; set; }
  public string? surname { get; set; }
  public string? full_name { get; set; }
}

public class Payee
{
  public string? email_address { get; set; }
  public string? merchant_id { get; set; }
}

public class Payer
{
  public string? email_address { get; set; }
  public string? payer_id { get; set; }
  public Address address { get; set; }
  public Name name { get; set; }
}

public class Payments
{
  public List<Capture> captures { get; set; }
}

public class PurchaseUnit
{
  public string? reference_id { get; set; }
  public Amount amount { get; set; }
  public Payee payee { get; set; }
  public Shipping shipping { get; set; }
  public Payments payments { get; set; }
}

public class CaptureOrder
{
  public DateTime create_time { get; set; }
  public DateTime update_time { get; set; }
  public string? id { get; set; }
  public string? intent { get; set; }
  public string? status { get; set; }
  public Payer payer { get; set; }
  public List<PurchaseUnit> purchase_units { get; set; }
  public List<Link> links { get; set; }
}

public class SellerProtection
{
  public string? status { get; set; }
  public List<string?> dispute_categories { get; set; }
}

public class Shipping
{
  public Name name { get; set; }
  public Address address { get; set; }
}
