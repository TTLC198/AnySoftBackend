using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RPM_Project_Backend.Helpers;

namespace RPM_Project_Backend.Domain;

public class ProductRequestDto
{
    [ValidateNever]
    public string? Name { get; set; }
    [ValidateNever]
    public MinMaxFloatObject? Rating { get; set; }
    [ValidateNever]
    public MinMaxFloatObject? Cost { get; set; }
    [ValidateNever]
    public MinMaxFloatObject? Discount { get; set; }
    [ValidateNever]
    public MinMaxDateTimeObject? PublicationDate { get; set; }
    [ValidateNever]
    public List<int>? Genres { get; set; }
    [ValidateNever]
    public List<int>? Properties { get; set; }
    [ValidateNever]
    public Dictionary<int, List<string>>? Attributes { get; set; }
    [ValidateNever]
    public SortOrder? Order { get; set; }
}

public class MinMaxFloatObject
{
    [ValidateNever]
    public float? Min { get; set; }
    [ValidateNever]
    public float? Max { get; set; }
}

public class MinMaxDateTimeObject
{
    [ValidateNever]
    [JsonConverter(typeof(JsonDateTimeConverter))]
    public DateTime? Min { get; set; }
    [ValidateNever]
    [JsonConverter(typeof(JsonDateTimeConverter))]
    public DateTime? Max { get; set; }
}

public class SortOrder
{
    [ValidateNever]
    public string? Type { get; set; }
    [ValidateNever]
    public int? Direction { get; set; } // 0 - up, 1 - down
}