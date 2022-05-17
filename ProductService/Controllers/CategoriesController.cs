using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.DAL;
using ProductService.Model;
using System.Security.Claims;

namespace ProductService.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IProductsRepository repository;
        public CategoriesController(IProductsRepository respository)
        {
            this.repository = respository;
        }

        [AllowAnonymous]
        [HttpGet("get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategories()
        {
            var res = await this.repository.ListCategories();
            return Ok(res);
        }

        [Authorize(Policy = "Admin")]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] string name) {
            Console.WriteLine("name:" + HttpContext?.User?.Identity?.Name);
            var result = await this.repository.AddCategory(name);
            return Ok(result);
        }
    }
}
