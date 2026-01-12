using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;

namespace price_web_api.Models;
public class PriceRecord
{
    public required int fundId { get; set; }

    [ForeignKey("fundId")]
    [JsonIgnore]
    [XmlIgnore]
    public virtual FundInfo fundData { get; set; }

    [JsonIgnore]
    public int Key => HashCode.Combine(fundId, date);

    [Column(TypeName = "decimal(8,2)")]
    public decimal close { get; set; }

    [Column(TypeName = "decimal(8,2)")]
    public decimal open { get; set; }

    [Column(TypeName = "decimal(8,2)")]
    public decimal high { get; set; }

    [Column(TypeName = "decimal(8,2)")]
    public decimal low { get; set; }

    [Column(TypeName = "decimal(8,2)")]
    public decimal? nonzeroprice => close > 0 ? close : open > 0 ? open : low > 0 ? low : high > 0 ? high : nav;

    // [Required(ErrorMessage = "nav required")]
    // [Range(0.1, double.MaxValue, ErrorMessage = "nav must be greater than 0")]
    [Column(TypeName = "decimal(8,2)")]
    public decimal nav { get; set; } // sometimes only nav as price is available

    public int volume { get; set; }

    [Required(ErrorMessage = "date is required")]
    public DateOnly date { get; set; }

    public override string ToString()
    {
        return $"PriceRecord [Key={Key}, fundId={fundId}, Date={date}, Close={close}, Open={open}, High={high}, Low={low}, NAV={nav}, Volume={volume}]";
    }
}