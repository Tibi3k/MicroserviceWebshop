using BasketService.Services;
using Microsoft.AspNetCore.Mvc;
using BasketService.DAL;
using BasketService.Model;

namespace BasketService.Controllers
{

    [ApiController]
    [Route("/api/basket")]
    public class BasketController : ControllerBase
    {
        private readonly IRabbitMQService rabbitMQService;
        private readonly IBasketRepository repository;

        public BasketController(IRabbitMQService rabbitMQ, IBasketRepository repository) { 
            this.rabbitMQService = rabbitMQ;
            this.repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<UserBasket>> listAll()  => repository.getAllBaskets();

    }
}