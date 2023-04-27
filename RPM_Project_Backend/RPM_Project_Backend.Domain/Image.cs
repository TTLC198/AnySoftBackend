using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace RPM_Project_Backend.Domain;

[Index(nameof(Id), IsUnique = true)]
public class Image
{
    [Key]
    [Column("img_id"), Required] 
    public int Id { get; set; }
    
    [Column("img_pro_id")] 
    public int? ProductId { get; set; }
    
    [Column("img_user_id")] 
    public int? UserId { get; set; }

    [Column("img_content_type"), Required, StringLength(50)]
    public string? ContentType { get; set; }

    [Column("img_path"), Required, StringLength(50)]
    public string? ImagePath { get; set; }

    [Column("img_ts"), Required]
    public DateTime Ts { get; set; }
}

public class ImageDto
{
    public int? ProductId { get; set; }
    
    public int UserId { get; set; }
    
    [Required]
    public IFormFile Image { get; set; }
}