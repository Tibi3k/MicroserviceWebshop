
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

        public async Task<IReadOnlyCollection<Product>> List()
        {
            var result = await db.Products.Include(p => p.Category).ToListAsync().WaitAsync(TimeSpan.FromSeconds(5));
            return result.Select(ToModel).ToList();
        }

        public async Task<Product?> FindById(int id) { 
            var dbProduct = await db.Products
                .Where(p => p.Id == id)
                .Include(p => p.Category)
                .SingleOrDefaultAsync();
            if (dbProduct == null)
                return null;
            return ToModel(dbProduct);
        }
        public async Task RemoveProductQuantity(int id, int quantity) {
            var dbProduct = await db.Products
                .Where(p => p.Id == id)
                .SingleOrDefaultAsync();
            dbProduct.Quantity -= quantity;
            await db.SaveChangesAsync();
        }

        public async Task<Product> AddNewProduct(CreateProduct newProduct)
        {
            using (var tran = db.Database.BeginTransaction(System.Data.IsolationLevel.RepeatableRead))
            {
                //Check if product exists
                var existingProduct = await db.Products
                    .Where(p => p.Name == newProduct.Name)
                    .FirstOrDefaultAsync();
                if (existingProduct != null)
                    throw new ArgumentException("Product already exists");

                //Create category if it does not exists
                var existingCategory = await db.Categories
                    .Where(c => c.Name == newProduct.Category)
                    .SingleOrDefaultAsync();
                if (existingCategory == null)
                {
                    var newCategory = new DbCategory() { Name = newProduct.Category };
                    await db.Categories.AddAsync(newCategory);
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
                await db.SaveChangesAsync();
                await tran.CommitAsync();
                return ToModel(toInsert);
            }
        }

        public async Task<Product?> DeleteProduct(int id) {
            var product = await db.Products
                .Where(p => p.Id == id)
                .Include(p => p.Category)
                .SingleOrDefaultAsync();

            if (product == null)
                return null;

            db.Products.Remove(product);
            await db.SaveChangesAsync();
            return ToModel(product);
        }


        public async Task<Product?> UpdateProduct(Product product) {
            using (var tran = db.Database.BeginTransaction(System.Data.IsolationLevel.RepeatableRead))
            {
                var dbProduct = await db.Products
                .Where(p => p.Id == product.Id)
                .SingleOrDefaultAsync();

                var category = await db.Categories
                    .Where(c => c.Name == product.Category)
                    .SingleOrDefaultAsync();

                if (dbProduct == null || category == null)
                {
                    return null;
                }

                dbProduct.Price = product.Price;
                dbProduct.Name = product.Name;
                dbProduct.Quantity = product.Quantity;
                dbProduct.Description = product.Description;
                dbProduct.Category = category;
                await db.SaveChangesAsync();
                await tran.CommitAsync();
                return ToModel(dbProduct);
            }
        }

        public async Task<IEnumerable<Category>> ListCategories() {
            return await this.db.Categories
                .Select(prod => new Category { 
                    Id = prod.Id,
                    Name = prod.Name
                })
                .ToListAsync(); 
        }

        public async Task AddQuantityToProduct(int productId, int quantity) {
            var dbProduct = await db.Products
                .Where(p => p.Id == productId)
                .SingleOrDefaultAsync();
            dbProduct.Quantity += quantity;
            await db.SaveChangesAsync();
        }

        public async Task<Category> AddCategory(string categoryName) {
            using (var tran = db.Database.BeginTransaction(System.Data.IsolationLevel.RepeatableRead))
            {
                var alreadyExists = await db.Categories.SingleOrDefaultAsync(c => c.Name == categoryName);
                if (alreadyExists != null)
                {
                    return new Category
                    {
                        Id = alreadyExists.Id,
                        Name = alreadyExists.Name
                    };
                }

                var dbCategory = new DbCategory
                {
                    Name = categoryName
                };
                await db.Categories.AddAsync(dbCategory);
                await db.SaveChangesAsync();
                await tran.CommitAsync();
                return new Category
                {
                    Id = dbCategory.Id,
                    Name = dbCategory.Name
                };
            }
        }

        private static Model.Product ToModel(DbProduct value)
        {
            return new Model.Product(value.Id, value.Name, value.Description, value.Price, value.Quantity, value.Category.Name);
        }
    }
}

