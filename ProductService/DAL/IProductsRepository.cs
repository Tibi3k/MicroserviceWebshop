using ProductService.Controllers.DTO;
using ProductService.Model;

namespace ProductService.DAL {
    public interface IProductsRepository {
        public IReadOnlyCollection<Product> List();
        public Product AddNewProduct(CreateProduct newProduct);
        public Product? DeleteProduct(int id);
        public Product? FindById(int id);
    }
}