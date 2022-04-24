using BasketService.Services;
using Microsoft.AspNetCore.Mvc;
using BasketService.DAL;
using BasketService.Model;
using static BasketService.Services.RabbitMQService;
using MassTransit;

namespace BasketService.Controllers
{

    [ApiController]
    [Route("/api/basket")]
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

        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserBasket>> GetUserBasket(int userId) {
            var basket = await repository.FindBasketByUserIdAsync(userId);
            if(basket == null)
                return NotFound("User doesn't has a basket");
            return Ok(basket);
        }

        [HttpDelete("{userId}/{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteProductFromBasket(int userId, int productId) {
            var modifiedLines = await repository.DeleteProductFromBasketAsync(userId, productId);
            if(modifiedLines > 0)
                return Ok($"Modified {modifiedLines} lines");
            return NoContent();
        }

        [HttpPost("{userId}/order")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> OrderCurrentBasket(int userId)
        {
            var basket = await repository.FindBasketByUserIdAsync(userId);
            if (basket == null)
                return NotFound("No user or basket");
            await this.rabbitMQ.ConvertBasketToOrderAsync(basket);
            await this.rabbitMQ.SendOrderConfirmationEmailAsync(basket);
            return Ok();
        }
    }
}