using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.Controllers.DTO;
using ProductService.DAL;
using ProductService.Model;
using ProductService.Services;
using System.Security.Claims;
using static ProductService.Services.RabbitMQService;

namespace ProductService.Controllers
{
    [Route("api/Products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsRepository repository;
        private readonly IRabbitMQService rabbitmq;
        public ProductsController(IProductsRepository respository, IRabbitMQService rabbitmq) {
            this.repository = respository;
            this.rabbitmq = rabbitmq;
        }

        [AllowAnonymous]
        [HttpGet("public")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Product>>> GetAllProducts() {
                var result = await this.repository.List().WaitAsync(TimeSpan.FromSeconds(5));
                return Ok(result.ToList());
        }

        [AllowAnonymous]
        [HttpGet("public/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> FindById(int id)
        {
            try
            {
                var product = await this.repository.FindById(id);
                if (product == null)
                    return NotFound();
                return Ok(product);
            }
            catch (Exception ex) {
                return Problem();
            }

        }

        [Authorize(Policy = "Admin")]
        [HttpPost("protected")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Product>> addProduct([FromBody] CreateProduct createProduct)
        {
                var product = await this.repository.AddNewProduct(createProduct);
                return CreatedAtAction(nameof(FindById), new { id = product.Id }, product);
        }

        [Authorize(Policy = "Admin")]
        [HttpDelete("protected/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> DeleteProduct(int id) {
            try {
                var result = await this.repository.DeleteProduct(id);
                if (result == null)
                    return NotFound();
                return NoContent();
            }
            catch (Exception ex) {
                return Problem();
            }
        }

        [Authorize(Policy = "Admin")]
        [HttpPut("protected")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Product>> UpdateProduct(Product product)
        {
            try
            {
                var result = await this.repository.UpdateProduct(product);
                if (result == null)
                    return BadRequest();
                return Ok();
            }
            catch (Exception ex) {
                return Problem();
            }

        }

        [Authorize(Policy = "User")]
        [HttpPost("protected/{productID}/tobasket")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddProductToBasket([FromQuery] int? amount, int productID) {
            var userID = getUserIdFromClaim(User);
            if (amount == null)
                return BadRequest("Amount not specified");
            var email = User.Claims.First(claim => claim.Type == "emails").Value;
            Product? product;
            try{
                product = await this.repository.FindById(productID).WaitAsync(TimeSpan.FromSeconds(5));
            }
            catch (Exception ex) {
                return Problem();
            }
            if (product == null)
                return BadRequest("Invalid productID");
            if (product.Quantity < amount)
                return BadRequest("Not enough products");
            product.Quantity = amount.Value;
            var result = await this.rabbitmq.AddProductToBasket(product, userID.ToString(), email);
            if (result)
            {
                await this.repository.RemoveProductQuantity(productID, amount.Value);
                return Ok();
            }
            return Problem("Something went wrong!");

        }
        private string getUserIdFromClaim(ClaimsPrincipal principal)
        {
            return principal.Claims
                .FirstOrDefault(claim =>
                 claim.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier")!
                .Value;
        }
    }

}
