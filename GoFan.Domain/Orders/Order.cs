namespace GoFan.Domain.Orders;

public class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public decimal TotalAmount { get; set; }

    public string ShippingProvince { get; set; } = null!;

    public string ShippingDistrict { get; set; } = null!;

    public string ShippingWard { get; set; } = null!;

    public string ShippingAddress { get; set; } = null!;

    public string? ShippingNote { get; set; }

    public string? OrderNote { get; set; }

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.COD;

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    public string? CancellationReason { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ConfirmedAt { get; set; }

    public DateTime? ShippingAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? CanceledAt { get; set; }
}