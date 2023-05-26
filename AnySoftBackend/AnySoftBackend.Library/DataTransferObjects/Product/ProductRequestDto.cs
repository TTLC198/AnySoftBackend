using System.Collections.Generic;
using System.Text.Json.Serialization;
using AnySoftBackend.Library.Misc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AnySoftBackend.Library.DataTransferObjects.Product;

public class ProductRequestDto
{
    [ValidateNever]
    [JsonPropertyName("ids")]
    public List<int>? Ids { get; set; }
    
    [ValidateNever]
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [ValidateNever]
    [JsonPropertyName("rating")]
    public MinMaxObject<float>? Rating { get; set; }
    [ValidateNever]
    [JsonPropertyName("cost")]
    public MinMaxObject<float>? Cost { get; set; }
    [ValidateNever]
    [JsonPropertyName("discount")]
    public MinMaxObject<float>? Discount { get; set; }
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
    public SortOrderObject? Order { get; set; }
}