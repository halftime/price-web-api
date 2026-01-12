using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;

namespace price_web_api.Models;
public class PriceRecord
{
    private decimal _close;
    private decimal _nav;
    private decimal _open;
    private decimal _high;
    private decimal _low;

    public required int fundId { get; set; }

    [ForeignKey("fundId")]
    [JsonIgnore]
    [XmlIgnore]
    public virtual FundInfo fundData { get; set; }

    [JsonIgnore]
    public int Key => HashCode.Combine(fundId, date);

    [Column(TypeName = "decimal(8,2)")]
    public decimal close
    {
        get => _close;
        set
        {
            _close = value;
            UpdateNonZeroPrice();
        }
    }

    [Column(TypeName = "decimal(8,2)")]
    public decimal open
    {
        get => _open;
        set
        {
            _open = value;
            UpdateNonZeroPrice();
        }
    }

    [Column(TypeName = "decimal(8,2)")]
    public decimal high
    {
        get => _high;
        set
        {
            _high = value;
            UpdateNonZeroPrice();
        }
    }

    [Column(TypeName = "decimal(8,2)")]
    public decimal low
    {
        get => _low;
        set
        {
            _low = value;
            UpdateNonZeroPrice();
        }
    }

    [Column(TypeName = "decimal(8,2)")]
    public decimal? nonzeroprice { get; private set; }

    [Column(TypeName = "decimal(8,2)")]
    public decimal nav
    {
        get => _nav;
        set
        {
            _nav = value;
            UpdateNonZeroPrice();
        }
    }

    public int volume { get; set; }

    [Required(ErrorMessage = "date is required")]
    public DateOnly date { get; set; }

    private void UpdateNonZeroPrice()
    {
        nonzeroprice = _close != 0 ? _close 
            : _open != 0 ? _open 
            : _high != 0 ? _high 
            : _low != 0 ? _low 
            : _nav != 0 ? _nav 
            : null;
    }

    public override string ToString()
    {
        return $"PriceRecord [Key={Key}, fundId={fundId}, Date={date}, Close={close}, Open={open}, High={high}, Low={low}, NAV={nav}, Volume={volume}]";
    }
}