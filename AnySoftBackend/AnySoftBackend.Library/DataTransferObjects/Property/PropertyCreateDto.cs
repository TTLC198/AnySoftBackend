namespace AnySoftBackend.Library.DataTransferObjects.Property;

/// <summary>
/// Property Data Transfer Object
/// </summary>
public class PropertyCreateDto
{
    /// <summary>
    /// Property Name
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// Property Product Id
    /// </summary>
    public int? ProductId { get; set; }
}