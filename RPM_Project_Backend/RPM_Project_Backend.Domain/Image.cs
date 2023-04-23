using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using RPM_PR_LIB.Enums;

namespace RPM_Project_Backend.Domain;

public class Image
{
    [Key]
    [Column("img_id"), Required] 
    public int Id { get; set; }
    
    [Column("img_res_id"), Required] 
    public int ResourceId { get; set; }
    
    [Column("img_type"), Required]
    public ImageType Type { get; set; }
    [Column("img_content_type"), Required]
    public string? ContentType { get; set; }
    
    [Column("img_description")]
    public string? Description { get; set; }

    [Column("img_path"), Required]
    public string? ImagePath { get; set; }

    [Column("img_ts"), Required]
    public DateTime Ts { get; set; }
}

public class ImageDto
{
    [Required]
    public int ResourceId { get; set; }
    [Required]
    [Range(0,1, ErrorMessage = "Wrong type")]
    public ImageType Type { get; set; }
    
    public string? Description { get; set; }
    [Required]
    public IFormFile Image { get; set; }
}