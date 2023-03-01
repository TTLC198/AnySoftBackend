﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_PR_LIB;

public class ProductRequest
{
    [ValidateNever]
    public string? Name { get; set; }
    [ValidateNever]
    public MinMaxObject? Rating { get; set; }
    [ValidateNever]
    public MinMaxObject? Cost { get; set; }
    [ValidateNever]
    public int? Discount { get; set; }
    [ValidateNever]
    public int? Category { get; set; }
    [ValidateNever]
    public Dictionary<int, List<string>>? Attributes { get; set; }
    [ValidateNever]
    public int? Quantity { get; set; }
    [ValidateNever]
    public SortOrder? Order { get; set; }
}

public class MinMaxObject
{
    [ValidateNever]
    public float? Min { get; set; }
    [ValidateNever]
    public float? Max { get; set; }
}

public class SortOrder
{
    [ValidateNever]
    public string? Type { get; set; }
    [ValidateNever]
    public int? Direction { get; set; } // 0 - up, 1 - down
}