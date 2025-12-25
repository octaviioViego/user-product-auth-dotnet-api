using System.ComponentModel.DataAnnotations;

public class GetUsersQuery
{
    [Range(1, int.MaxValue)]
    public int? Page { get; set; }

    [Range(1, 100)]
    public int? Limit { get; set; }

    [RegularExpression("asc|desc")]
    public string? Order { get; set; }

    [AllowedValues(
        "username","email","date_joined","first_name","last_name",
        "is_active","is_deleted","is_superuser","email_verified",
        "date_of_birth","nationality","occupation","gender","role"
    )]
    public string? Sort { get; set; }

    public bool? IsActive { get; set; }
    public bool? IsDeleted { get; set; }
    public bool? IsSuperuser { get; set; }
    public bool? EmailVerified { get; set; }
}
