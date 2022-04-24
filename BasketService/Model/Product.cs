namespace BasketService.Model
{
    public class Product
    {
        public Product() { }
        public Product(int Id, string Name, string Description, int Price, int Quantity, string Category)
        {
            this.Id = Id;
            this.Name = Name;
            this.Description = Description;
            this.Price = Price;
            this.Quantity = Quantity;
            this.Category = Category;
        }

        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = string.Empty;
        public int Price { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }

        public override string ToString()
        {
            return $"Id: {this.Id}, Name: {this.Name}";
        }

    }
}
