using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RPM_Project_Backend.Helpers;

namespace RPM_Project_Backend.Domain;

public class ProductRequestDto
{
    [ValidateNever]
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [ValidateNever]
    [JsonPropertyName("rating")]
    public MinMaxFloatObject? Rating { get; set; }
    [ValidateNever]
    [JsonPropertyName("cost")]
    public MinMaxFloatObject? Cost { get; set; }
    [ValidateNever]
    [JsonPropertyName("discount")]
    public MinMaxFloatObject? Discount { get; set; }
    [ValidateNever]
    [JsonPropertyName("publicationDate")]
    public MinMaxDateTimeObject? PublicationDate { get; set; }
    [ValidateNever]
    [JsonPropertyName("genres")]
    public List<int>? Genres { get; set; }
    [ValidateNever]
    [JsonPropertyName("properties")]
    public List<int>? Properties { get; set; }
    [ValidateNever]
    [JsonPropertyName("attributes")]
    public Dictionary<int, List<string>>? Attributes { get; set; }
    [ValidateNever]
    [JsonPropertyName("order")]
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