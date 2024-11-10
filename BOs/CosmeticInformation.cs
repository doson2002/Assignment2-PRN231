using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BOs;

public partial class CosmeticInformation
{
    [Key]
    public string CosmeticId { get; set; } = null!;

    [Required]
    [RegularExpression(@"^([A-Z][a-zA-Z0-9@#( )]*)$", ErrorMessage = "Invalid format for CosmeticName.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "CosmeticName must be between 2 and 80 characters.")]
    public string CosmeticName { get; set; } = null!;

    [Required]
    public string SkinType { get; set; } = null!;
    [Required]
    public string ExpirationDate { get; set; } = null!;
    [Required]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "CosmeticSize must be between 2 and 80 characters.")]
    public string CosmeticSize { get; set; } = null!;
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Dollar Price must be > 0")]
    public decimal DollarPrice { get; set; }
    [Required]
    public string? CategoryId { get; set; }

    public virtual CosmeticCategory? Category { get; set; }
}
