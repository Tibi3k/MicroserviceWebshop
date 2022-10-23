namespace PaymentService.Models;

public class Approve
{
    public bool accelerated { get; set; }
    public string orderID { get; set; }
    public string payerID { get; set; }
    public string? paymentID { get; set; }
    public string? billingToken { get; set; }
    public string facilitatorAccessToken { get; set; }
    public string paymentSource { get; set; }
}
