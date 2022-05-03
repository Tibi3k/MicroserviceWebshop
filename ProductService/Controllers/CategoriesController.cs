using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.DAL;
using ProductService.Model;

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


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Category>> GetAllCategories()
        {
            return Ok(this.repository.ListCategories());
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<Category> CreateCategory([FromBody] string name) {
            var result = this.repository.AddCategory(name);
            return Ok(result);
        }
    }
}
