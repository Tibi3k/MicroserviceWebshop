using System.ComponentModel.DataAnnotations;

namespace ProductService.DAL.EfDbContext
{
    public class DbCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public List<DbProduct> Products { get; set; } = new List<DbProduct>();
    }
}
