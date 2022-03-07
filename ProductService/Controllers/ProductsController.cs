﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.Controllers.DTO;
using ProductService.DAL;
using ProductService.Model;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsRepository repository;
        public ProductsController(IProductsRepository respository) {
            this.repository = respository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<String> feckOff() {
            return Ok(this.repository.List());
        }

        [HttpGet("hello")]
        public ActionResult<String> hello()
        {
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
    }
}
