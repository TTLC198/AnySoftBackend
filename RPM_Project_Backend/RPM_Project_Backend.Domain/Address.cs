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
    [Column("ad_street"), Required]
    public string Street { get; set; } = null!;
    [Column("ad_city"), Required]
    public string City { get; set; } = null!;
    [Column("ad_state"), Required]
    public string State { get; set; } = null!;
    [Column("ad_country"), Required]
    public string Country { get; set; } = null!;
    [Column("ad_u_id"), Required]
    public int UserId { get; set; }
    [Column("ad_zip"), Required]
    public string Zip { get; set; }
    [Column("ad_is_active"), Required]
    public bool IsActive { get; set; }
    
    [JsonIgnore]
    [NotMapped]
    public virtual IEnumerable<Order> Orders { get; } = new List<Order>();
    [ValidateNever]
    [JsonIgnore]
    [NotMapped]
    public virtual User User { get; set; } = null!;
}
