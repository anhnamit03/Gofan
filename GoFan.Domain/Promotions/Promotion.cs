using System;
using System.Collections.Generic;
using System.Text;

namespace GoFan.Domain.Promotions
{
    public class Promotion
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal DiscountPercent { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public bool Active { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}
