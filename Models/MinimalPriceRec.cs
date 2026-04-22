using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;

namespace price_web_api.Models;

public class MinimalPriceRec

{
    // [Required]
    // public required int InvestmentId { get; set; }

    // [ForeignKey("InvestmentId")]
    // [JsonIgnore]
    // [XmlIgnore]
    // public virtual Investment? Investment { get; set; }

    [Required]
    public required string Symbol { get; set; } = string.Empty;

    [Column(TypeName = "decimal(8,2)")]
    [Required(ErrorMessage = "price is required")]
    public required decimal price {get; set; }// some records have price as null but nav as non-null

    [Required(ErrorMessage = "date is required")]
    [XmlIgnore]
    public required DateOnly date { get; set; }

    [NotMapped]
    [JsonIgnore]
    [XmlElement("date")]
    public string DateString
    {
        get => date.ToString("yyyy-MM-dd");
        set => date = DateOnly.Parse(value);
    }


}