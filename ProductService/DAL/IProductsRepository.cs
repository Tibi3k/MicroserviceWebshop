using ProductService.Controllers.DTO;
using ProductService.Model;

namespace ProductService.DAL {
    public interface IProductsRepository {
        public Task<IReadOnlyCollection<Product>> List();
        public Task<Product> AddNewProduct(CreateProduct newProduct);
        public Task<Product?> DeleteProduct(int id);
        public Task<Product?> FindById(int id);
        public Task<Product?> UpdateProduct(Product product);
        public Task<Category> AddCategory(string categoryName);
        public Task<IEnumerable<Category>> ListCategories();
        public Task RemoveProductQuantity(int id, int quantity);
        public Task AddQuantityToProduct(int productId, int quantity);
    }
}