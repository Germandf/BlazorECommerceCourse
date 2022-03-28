using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorECommerceCourse.Shared;
public class ProductSearchResultDto
{
    public List<Product> Products { get; set; } = new();
    public int Pages { get; set; }
    public int CurrentPage { get; set; }
}
