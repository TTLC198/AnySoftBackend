using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using RPM_Project_Backend.Enums;

namespace RPM_PR_LIB;

public class Image
{
    [Column("img_id"), Required] 
    public int Id { get; set; }
    
    [Column("img_res_id"), Required] 
    public int ResourceId { get; set; }
    
    [Column("img_type"), Required]
    public ImageType Type { get; set; }
    [Column("img_content_type"), Required]
    public string ContentType { get; set; }
    
    [Column("img_description")]
    public string Description { get; set; }

    [Column("img_path"), Required]
    public string ImagePath { get; set; }

    [Column("img_ts"), Required]
    public DateTime Ts { get; set; }
}

public class ImageDto
{
    public int ResourceId { get; set; }
    public ImageType Type { get; set; }
    public string Description { get; set; }
    public IFormFile Image { get; set; }
}