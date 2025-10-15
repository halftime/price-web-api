using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace price_web_api.Models;
    public class PriceRecord
{
    public required int FundId { get; set; }

    [ForeignKey("FundId")]
    public required virtual FundInfo FundData { get; set; }

    public required decimal Close { get; set; } 

    public decimal Open { get; set; }

    public decimal High { get; set; }

    public decimal Low { get; set; }

    public decimal Volume { get; set; }
    public required DateOnly Date { get; set; }

    // [Key]
    // public string RecordKey => $"{this.FundData.BloombergTicker}|{this.Date.ToString("dd-MM-yyyy")}";
}