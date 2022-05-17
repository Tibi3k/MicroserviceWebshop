using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.DAL;
using ProductService.Model;
using System.Security.Claims;
using System.Text;

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

        [HttpGet("get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategories()
        {
            var res = await this.repository.ListCategories();
            return Ok(res);
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] string name) {
            var role = decodeUserData("jobTitle");
            if (role != "Admin")
                return Unauthorized();
            var result = await this.repository.AddCategory(name);
            return Ok(result);
        }

        private string decodeUserData(string data)
        {
            Console.WriteLine("res:" + Request.Headers[data]);
            var encodedUserData = Request.Headers[data].ToString() ?? "";
            return Encoding.UTF8.GetString(Convert.FromBase64String(encodedUserData));
        }
    }
}
