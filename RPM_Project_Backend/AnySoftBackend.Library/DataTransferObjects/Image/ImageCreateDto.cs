using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace AnySoftBackend.Library.DataTransferObjects.Image;

public class ImageCreateDto
{
    public int? ProductId { get; set; }
    
    public int UserId { get; set; }

    [Required] public IFormFile Image { get; set; }
}