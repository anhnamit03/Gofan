using System;

namespace GoFan.Application.DTOs.Promotions;

public class PromotionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal DiscountPercent { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public bool Active { get; set; }
}
