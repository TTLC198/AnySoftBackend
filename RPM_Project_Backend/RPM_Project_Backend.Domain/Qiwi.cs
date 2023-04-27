using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RPM_Project_Backend.Domain;

public class Qiwi
{
    [Key]
    [Column("qiwi_id"), Required]
    public int Id { get; set; }
    
    [Column("qiwi_number"), Required]
    public int Number { get; set; }
    
    [Column("qiwi_pay_id"), Required]
    public int PaymentId { get; set; }
    
    [ValidateNever]
    public virtual Payment? Payment { get; set; }
}
