namespace ProductService.DAL.EfDbContext {
    public class DbProduct {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Price { get; set; }
        public int Quantity { get; set; }

        public string Description { get; set; } = "";
        public DbCategory Category { get; set;}
        public int CategoryId { get; set; }
    }
}