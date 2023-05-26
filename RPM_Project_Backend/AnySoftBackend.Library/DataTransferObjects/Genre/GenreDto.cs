namespace AnySoftBackend.Library.DataTransferObjects;

/// <summary>
/// Genre Data Transfer Object
/// </summary>
public class GenreDto
{
    /// <summary>
    /// Genre Name
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// Genre Product Id
    /// </summary>
    public int? ProductId { get; set; }
}