namespace PaymentService.Models;

public class BillingAddress
{
  public string address_line_1 { get; set; }
  public string address_line_2 { get; set; }
  public string admin_area_1 { get; set; }
  public string admin_area_2 { get; set; }
  public string postal_code { get; set; }
  public string country_code { get; set; }
}

public class Card
{
  public string number { get; set; }
  public string expiry { get; set; }
  public string name { get; set; }
  public BillingAddress billing_address { get; set; }
}

public class PaymentSource
{
  public Card card { get; set; }
  public StoredCredential stored_credential { get; set; }
}

public class PreviousNetworkTransactionReference
{
  public string id { get; set; }
  public string network { get; set; }
}
public class Order
{
  public string intent { get; set; }
  public List<PurchaseUnit> purchase_units { get; set; }
  public PaymentSource payment_source { get; set; }
}

public class StoredCredential
{
  public string payment_initiator { get; set; }
  public string payment_type { get; set; }
  public string usage { get; set; }
  public PreviousNetworkTransactionReference previous_network_transaction_reference { get; set; }
}
