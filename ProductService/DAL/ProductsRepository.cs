
using Microsoft.EntityFrameworkCore;
using ProductService.Controllers.DTO;
using ProductService.DAL.EfDbContext;
using ProductService.Model;

namespace ProductService.DAL {
    public class ProductsRepository : IProductsRepository
    {
        private readonly ProductDbContext db;

        public ProductsRepository(ProductDbContext db) {
            this.db = db;
        } 

        public IReadOnlyCollection<Product> List()
        {
            return db.Products.Include(p => p.Category).Select(ToModel).ToList();
        }

        public Product? FindById(int id) { 
            var dbProduct = db.Products
                .Where(p => p.Id == id)
                .Include(p => p.Category)
                .SingleOrDefault();
            if (dbProduct == null)
                return null;
            return ToModel(dbProduct);
        }

        public Product AddNewProduct(CreateProduct newProduct) {
            using (var tran = db.Database.BeginTransaction(System.Data.IsolationLevel.RepeatableRead)) {

                //Check if product exists
                var existingProduct = db.Products
                    .Where(p => p.Name == newProduct.Name)
                    .FirstOrDefault();
                if (existingProduct != null)
                    throw new ArgumentException("Product already exists");

                //Create category if it does not exists
                var existingCategory = db.Categories
                    .Where(c => c.Name == newProduct.Category)
                    .SingleOrDefault();
                if (existingCategory == null) {
                    var newCategory = new DbCategory() { Name = newProduct.Category };
                    db.Categories.Add(newCategory);
                    db.SaveChanges();
                    existingCategory = newCategory;
                }

                //Insert the new product
                var toInsert = new DbProduct
                {
                    Name = newProduct.Name,
                    Category = existingCategory,
                    Price = newProduct.Price,
                    Quantity = newProduct.Quantity,
                    Description = newProduct.Description,
                };
                db.Products.Add(toInsert);
                db.SaveChanges();
                tran.Commit();
                return ToModel(toInsert);
            }
        }

        public Product? DeleteProduct(int id) {
            var product = db.Products
                .Where(p => p.Id == id)
                .Include(p => p.Category)
                .SingleOrDefault();

            if (product == null)
                return null;

            db.Products.Remove(product);
            db.SaveChanges();
            return ToModel(product);
        }

        private static Model.Product ToModel(DbProduct value)
        {
            return new Model.Product(value.Id, value.Name, value.Description, value.Price, value.Quantity, value.Category.Name);
        }
    }
}

