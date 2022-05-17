using System.ComponentModel.DataAnnotations;

namespace ProductService.Model;

public class Category
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = "";
}
