using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;   

namespace price_web_api.Models;
public class Fund : Investment
{
    // Id, Symbol, Name are inherited from Investment

    [Required]
    [MaxLength(50)]
    public required string MorningStarId { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public required string ISIN { get; set; } = string.Empty;
}