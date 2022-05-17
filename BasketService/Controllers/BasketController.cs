using BasketService.Services;
using Microsoft.AspNetCore.Mvc;
using BasketService.DAL;
using BasketService.Model;
using static BasketService.Services.RabbitMQService;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text;

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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserBasket>>> GetAllBaskets() => await repository.getAllBasketsAsync();

        [HttpGet("userbasket")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<UserBasket>> GetUserBasket() {
            var userID = decodeUserData("UserId");
            var basket = await repository.FindBasketByUserIdAsync(userID);
            if(basket == null)
                return NoContent();
            return Ok(basket);
        }

        [HttpDelete("{productSubId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteProductFromBasket( string productSubId) {
            var userID = decodeUserData("UserId");
            var product = await repository.FindQuantityByBasketSubId(productSubId, userID);
            Console.WriteLine("returning product" + product?.Quantity ?? "null");
            bool result = false;
            if (product != null)
                result = await rabbitMQ.ReturnAvailableAmountToProduct(product.Id, product.Quantity);
            if (result)
            {
                await repository.DeleteProductFromBasketAsync(userID, productSubId);
                return Ok();
            }
            return NoContent();
        }

        [HttpPost("order")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> OrderCurrentBasket()
        {
            var userID = decodeUserData("UserId");
            var name = decodeUserData("UserName");
            var basket = await repository.FindBasketByUserIdAsync(userID);
            if (basket == null)
                return NotFound("No user or basket");
            var result1 = await this.rabbitMQ.ConvertBasketToOrderAsync(basket, name);
            var result2 = await this.rabbitMQ.SendOrderConfirmationEmailAsync(basket, name);
            if (result1 && result2) {
                await repository.ClearBasket(userID);
                return Ok();
            }
            return Problem();
        }

        [HttpGet("size")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> GetBasketSize() {
            var userID = decodeUserData("UserId");
            var result = await this.repository.GetBasketSize(userID);
            return Ok(result);
        }

        private string decodeUserData(string data) {
            Console.WriteLine("res:" + Request.Headers[data]);
            var encodedUserData = Request.Headers[data].ToString() ?? "";
            return Encoding.UTF8.GetString(Convert.FromBase64String(encodedUserData));
        }
    }
}