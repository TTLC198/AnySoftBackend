﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace AnySoftBackend.Domain;

/// <summary>
/// User Object
/// </summary>
[Index(nameof(Id), IsUnique = true)]
public class User
{
    /// <summary>
    /// Identifier
    /// </summary>
    [Key]
    [Column("u_id"), Required]
    public int Id { get; set; }
    /// <summary>
    /// Login
    /// </summary>
    [Column("u_login"), Required, StringLength(256)]
    public string? Login { get; set; }
    /// <summary>
    /// Password
    /// </summary>
    [Column("u_password"), Required, StringLength(256)]
    public string? Password { get; set; }
    /// <summary>
    /// Email
    /// </summary>
    [Column("u_email"), Required, StringLength(256)]
    public string? Email { get; set; }
    /// <summary>
    /// Role identifier
    /// </summary>
    [Column("u_role_id"), Required]
    public int RoleId { get; set; }
    /// <summary>
    /// Role Object
    /// </summary>
    [ForeignKey("RoleId")]
    public virtual Role? Role { get; set; }
    /// <summary>
    /// Product images
    /// </summary>
    [ValidateNever]
    public virtual IEnumerable<Image>? Images { get; }
    /// <summary>
    /// User payments
    /// </summary>
    [ValidateNever]
    public virtual IEnumerable<Payment>? Payments { get; }
    /// <summary>
    /// User reviews
    /// </summary>
    [ValidateNever]
    [NotMapped]
    public virtual IEnumerable<Review>? Reviews { get; }
    /// <summary>
    /// User reviews
    /// </summary>
    [ValidateNever]
    public virtual IEnumerable<UsersHaveProducts>? UsersHaveProducts { get; }
    /// <summary>
    /// User orders
    /// </summary>
    [ValidateNever]
    public virtual IEnumerable<Order>? Orders { get; }
}