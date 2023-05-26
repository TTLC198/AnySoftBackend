namespace AnySoftBackend.Library.DataTransferObjects.Genre;

/// <summary>
/// Genre Data Transfer Object
/// </summary>
public class GenreCreateDto
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