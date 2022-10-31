using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorECommerceCourse.Shared;
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Url { get; set; } = null!;
    public bool Visible { get; set; } = true;
    public bool Deleted { get; set; } = false;
    [NotMapped]
    public bool Editing { get; set; } = false;
    [NotMapped]
    public bool IsNew { get; set; } = false;
}
