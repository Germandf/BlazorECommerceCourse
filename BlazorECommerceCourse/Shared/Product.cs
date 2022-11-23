using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorECommerceCourse.Shared;

public class Product
{
    public int Id { get; set; }
    [Required]
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public Category Category { get; set; } = null!;
    public int CategoryId { get; set; }
    public bool Featured { get; set; }
    public List<ProductVariant> Variants { get; set; } = new();
    public bool Visible { get; set; } = true;
    public bool Deleted { get; set; } = false;
    [NotMapped]
    public bool Editing { get; set; } = false;
    [NotMapped]
    public bool IsNew { get; set; } = false;
}
