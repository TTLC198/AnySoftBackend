﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AnySoftBackend.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AnySoftBackend.Domain;

public class ProductsHaveGenres
{
    [Key]
    [Column("phg_id"), Required]
    public int Id { get; set; }

    [Column("phg_pro_id"), Required]
    public int ProductId { get; set; }

    [Column("phg_gen_id"), Required]
    public int GenreId { get; set; }

    [ValidateNever]
    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; }
    [ValidateNever]
    [ForeignKey("GenreId")]
    public virtual Genre? Genre { get; set; }
}
