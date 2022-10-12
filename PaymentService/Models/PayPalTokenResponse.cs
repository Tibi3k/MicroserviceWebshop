namespace PaymentService.Models;

public class PayPalTokenResponse
{
  public string Scope { get; set; }
  public string Access_token { get; set; }
  public string Token_type { get; set; }
  public string App_id { get; set; }
  public int Expires_in { get; set; }
  public string Nonce { get; set; }
}

