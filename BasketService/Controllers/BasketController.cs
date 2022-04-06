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

        public BasketController(IBasketRepository repository) { 
            this.repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<UserBasket>> listAll()  => repository.getAllBaskets();

        [HttpGet("add")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> addNew() {
            return Ok();
        }


    }
}