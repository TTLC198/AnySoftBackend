using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_Project_Backend.Domain;

public class Address
{
    [Key]
    [Column("ad_id"), Required]
    public int Id { get; set; }
    [Column("ad_street"), Required, StringLength(50)]
    public string? Street { get; set; }
    [Column("ad_city"), Required, StringLength(50)]
    public string? City { get; set; }
    [Column("ad_state"), Required, StringLength(50)]
    public string? State { get; set; }
    [Column("ad_country"), Required, StringLength(50)]
    public string? Country { get; set; }
    [Column("ad_u_id"), Required]
    public int UserId { get; set; }
    [Column("ad_zip"), Required, StringLength(20)]
    public string? Zip { get; set; }
    [Column("ad_is_active"), Required]
    public bool IsActive { get; set; }
    
    [JsonIgnore]
    public virtual IEnumerable<Order>? Orders { get; }
    [ValidateNever]
    [ForeignKey("ad_u_id")]
    public virtual User? User { get; set; }
}
