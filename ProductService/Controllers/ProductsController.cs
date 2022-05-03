﻿using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.Controllers.DTO;
using ProductService.DAL;
using ProductService.Model;
using ProductService.Services;
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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Product>> GetAllProducts() {
            Console.WriteLine(HttpContext.Request.Headers.ToString());
            Console.WriteLine("Token:" + HttpContext.Request.Headers["Authorization"]);
            Console.WriteLine("ContentType:" + HttpContext.Request.Headers["Content-Type"]);
            HttpContext.Response.Headers.Add("Authorization", HttpContext.Request.Headers["Authorization"]);
            return Ok(this.repository.List());
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

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Product> UpdateProduct(Product product)
        {
            var result = this.repository.UpdateProduct(product);
            if (result == null)
                return BadRequest();
            return Ok();
        }

        [HttpPost("{productID}/tobasket")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddProductToBasket([FromQuery] int? userID, [FromQuery] int? amount, int productID) {
            if (userID == null)
                return BadRequest("User not specified");
            if (amount == null)
                return BadRequest("Amount not specified");
            Console.WriteLine("Product id:" + productID);
            var product = this.repository.FindById(productID);
            if (product == null)
                return BadRequest("Invalid productID");
            if (product.Quantity < amount)
                return BadRequest("Not enough products");
            product.Quantity = amount.Value;
            await this.rabbitmq.AddProductToBasket(product, userID.Value);
            return Accepted();

        }
    }
}
