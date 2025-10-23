using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace price_web_api.Models;
    public class PriceRecord
{
    public int Key => HashCode.Combine(fundId, date);

    public required int fundId { get; set; }

    [ForeignKey("fundId")]
    public virtual FundInfo fundData { get; set; }

    [Column(TypeName = "decimal(8,2)")]
    public decimal close { get; set; }

    [Column(TypeName = "decimal(8,2)")]
    public decimal open { get; set; }

    [Column(TypeName = "decimal(8,2)")]
    public decimal high { get; set; }

    [Column(TypeName = "decimal(8,2)")]
    public decimal low { get; set; }

   [Column(TypeName = "decimal(8,2)")]
    public decimal nav { get; set; }

    public int volume { get; set; }
    public required DateOnly date { get; set; }

    // [Key]
    // public string RecordKey => $"{this.FundData.BloombergTicker}|{this.Date.ToString("dd-MM-yyyy")}";

    public override string ToString()
    {
        return $"PriceRecord [Key={Key}, fundId={fundId}, Date={date}, Close={close}, Open={open}, High={high}, Low={low}, NAV={nav}, Volume={volume}]";
    }
}