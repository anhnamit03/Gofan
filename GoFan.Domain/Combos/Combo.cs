using System;
using System.Collections.Generic;
using System.Text;

namespace GoFan.Domain.Combos
{
    public class Combo
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool Active { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
