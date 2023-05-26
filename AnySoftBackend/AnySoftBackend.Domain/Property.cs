﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace AnySoftBackend.Domain;

[Index(nameof(Id), IsUnique = true)]
public class Property
{
    [Key]
    [Column("prp_id"), Required]
    public int Id { get; set; }
    [Column("prp_name"), Required, StringLength(50)]
    public string? Name { get; set; }
    [Column("prp_icon"), Required, StringLength(50)]
    public string? Icon { get; set; }

    [ValidateNever]
    [JsonIgnore]
    public virtual IEnumerable<ProductsHaveProperties>? ProductsHaveProperties { get; }
}