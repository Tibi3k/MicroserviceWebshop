namespace ProductService.Controllers.DTO
{
    public class CreateProduct
    {
        public CreateProduct( string name, string description, int price, int quantity, string category)
        {
            Name = name;
            Description = description;
            Price = price;
            Quantity = quantity;
            Category = category;
        }

        public string Name { get; set; } = "";
        public string Description { get; set; } = string.Empty;
        public int Price { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }
    }
}
