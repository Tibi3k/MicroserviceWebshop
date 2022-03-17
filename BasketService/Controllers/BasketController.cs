using BasketService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BasketService.Controllers
{

    [ApiController]
    [Route("/api/basket")]
    public class BasketController : ControllerBase
    {
        public readonly IRabbitMQService rabbitMQService;

        public BasketController(IRabbitMQService rabbitMQ) { 
            this.rabbitMQService = rabbitMQ;
        }
    }
}