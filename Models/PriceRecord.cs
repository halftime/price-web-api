using System;

namespace price_web_api.Models
{
    public class PriceRecord
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public DateTime Date { get; set; }
    }
}