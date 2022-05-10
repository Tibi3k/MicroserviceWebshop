using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using OrderService.DAL.DbModel;
using OrderService.Model;
using System.Security.Claims;

namespace OrderService.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly IOrderRepository repository;

    public OrderController(ILogger<OrderController> logger, IOrderRepository repository)
    {
        this.repository = repository;
        _logger = logger;
    }

    [Authorize(Policy = "User")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<Order>> getOrdersOfUser() {
        var userId = getUserIdFromClaim(User);
        var order = await this.repository.GetOrdersOfUser(userId);
        if (order == null)
            return NoContent();
        return Ok(order);
    }

    private string getUserIdFromClaim(ClaimsPrincipal principal)
    {
        return principal.Claims
            .FirstOrDefault(claim =>
             claim.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier")!
            .Value;
    }

}
