﻿using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorECommerceCourse.Shared;
public class ProductType
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    [NotMapped]
    public bool Editing { get; set; } = false;
    [NotMapped]
    public bool IsNew { get; set; } = false;
}
