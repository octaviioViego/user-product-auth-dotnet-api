using System.ComponentModel.DataAnnotations;

public class ProductsQueryDTO
{
    [Range(1, int.MaxValue)]
    public int? Page { get; set; }

    [Range(1, 100)]
    public int? Limit { get; set; }

    [RegularExpression("asc|desc")]
    public string? Order { get; set; }

    [RegularExpression("^(name|price|created_at|type|status)?$")]
    public string? Sort { get; set; }
    public bool? Status { get; set;}
    public bool? isDeleted { get; set;}
    public string? Type { get; set;}
}