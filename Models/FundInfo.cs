using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace price_web_api.Models;
public class FundInfo
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public required int Id { get; set; }
    public required string BloombergTicker { get; set; }
    public required string Name { get; set; }
    public ICollection<PriceRecord> PriceRecords { get; set; } = new List<PriceRecord>();
}