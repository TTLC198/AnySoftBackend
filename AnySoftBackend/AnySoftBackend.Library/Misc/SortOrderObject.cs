using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AnySoftBackend.Library.Misc;

public class SortOrderObject
{
    [ValidateNever]
    public string? Type { get; set; }
    [ValidateNever]
    public int? Direction { get; set; }
}