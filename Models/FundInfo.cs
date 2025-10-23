using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;   

namespace price_web_api.Models;
public class FundInfo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public required int fundId { get; set; }
    public required string bloombergTicker { get; set; }
    public required string fundName { get; set; }

    public required string morningStarId { get; set; }
    public required string iSIN { get; set; }

    [JsonIgnore]
    public ICollection<PriceRecord> priceRecords { get; set; } = new List<PriceRecord>();
}