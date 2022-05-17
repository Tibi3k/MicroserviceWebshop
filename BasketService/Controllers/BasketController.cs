using BasketService.Services;
using Microsoft.AspNetCore.Mvc;
using BasketService.DAL;
using BasketService.Model;
using static BasketService.Services.RabbitMQService;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BasketService.Controllers
{
    [Route("/api/basket")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository repository;
        private readonly IRabbitMQService rabbitMQ;

        public BasketController(IBasketRepository repository, IRabbitMQService rabbitMQ) {
            this.repository = repository;
            this.rabbitMQ = rabbitMQ;
        }

        [Authorize(Policy = "Admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserBasket>>> GetAllBaskets() => await repository.getAllBasketsAsync();

        [Authorize(Policy = "User")]
        [HttpGet("userbasket")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<UserBasket>> GetUserBasket() {
            var userID = getUserIdFromClaim(User);
            var basket = await repository.FindBasketByUserIdAsync(userID);
            if(basket == null)
                return NoContent();
            return Ok(basket);
        }

        [Authorize(Policy = "User")]
        [HttpDelete("{productSubId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteProductFromBasket( string productSubId) {
            var userID = getUserIdFromClaim(User);
            var product = await repository.FindQuantityByBasketSubId(productSubId, userID);
            Console.WriteLine("returning product" + product?.Quantity ?? "null");
            if (product != null)
                await rabbitMQ.ReturnAvailableAmountToProduct(product.Id, product.Quantity);
            var modifiedLines = await repository.DeleteProductFromBasketAsync(userID, productSubId);
            if (modifiedLines > 0)
                return Ok($"Modified {modifiedLines} lines");
            return NoContent();
        }

        [Authorize(Policy = "User")]
        [HttpPost("order")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> OrderCurrentBasket()
        {
            var userID = getUserIdFromClaim(User);
            var surname = User.Claims.First(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname").Value;
            var name = User.Claims.First(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname").Value;
            var basket = await repository.FindBasketByUserIdAsync(userID);
            if (basket == null)
                return NotFound("No user or basket");
            await this.rabbitMQ.ConvertBasketToOrderAsync(basket, name + surname);
            await this.rabbitMQ.SendOrderConfirmationEmailAsync(basket, name + " "+ surname);
            await repository.ClearBasket(userID);
            return Ok();
        }

        [Authorize(Policy = "User")]
        [HttpGet("size")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> GetBasketSize() {
            var userId = getUserIdFromClaim(User);
            var result = await this.repository.GetBasketSize(userId);
            return Ok(result);
        }

        /// <summary>
        /// only call if user has been authorized
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        private string getUserIdFromClaim(ClaimsPrincipal principal) {
            return principal.Claims
                .FirstOrDefault(claim =>
                claim.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier")!
                .Value;
        }
    }
}