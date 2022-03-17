using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.Controllers.DTO;
using ProductService.DAL;
using ProductService.Model;
using ProductService.Services;

namespace ProductService.Controllers
{
    [Route("api/Products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsRepository repository;
        private readonly IRabbitMQService rabbitMQ;
        public ProductsController(IProductsRepository respository, IRabbitMQService rabbitMQ) {
            this.repository = respository;
            this.rabbitMQ = rabbitMQ;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<String> feckOff() {
            return Ok(this.repository.List());
        }

        [HttpGet("hello")]
        public ActionResult<String> hello()
        {
            this.rabbitMQ.Send();
            return Ok("hello");
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Product> FindById(int id)
        {
            var product = this.repository.FindById(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Product> addProduct([FromBody] CreateProduct createProduct)
        {
            try
            {
                var product = this.repository.AddNewProduct(createProduct);
                return CreatedAtAction(nameof(FindById), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Product> DeleteProduct(int id) {
            var result = this.repository.DeleteProduct(id);
            if (result == null)
                return NotFound();
            return NoContent();
        }

        [HttpPost("{productID}/tobasket")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult AddProductToBasket([FromQuery] string userID, [FromQuery] int? amount, int productID) {
            if (userID == null)
                return BadRequest("User not specified");
            if (amount == null)
                return BadRequest("Amount not specified");

            var product = this.repository.FindById(productID);
            if (product == null)
                return BadRequest("Invalid productID");
            if (product.Quantity < amount)
                return BadRequest("Not enough products");
            product.Quantity = amount.Value;
            this.rabbitMQ.AddProductToBasket(product);
            return Accepted();

        }
    }
}
