using System.ComponentModel.DataAnnotations;

namespace GoFan.Application.DTOs.Carts;

public class AddToCartDto
{
    [Required(ErrorMessage = "ProductId là bắt buộc.")]
    public int ProductId { get; set; }

    [Range(1, 999, ErrorMessage = "Số lượng phải từ 1 đến 999.")]
    public int Quantity { get; set; } = 1;

    public List<int>? AddOnProductIds { get; set; }
}

public class UpdateCartItemQuantityDto
{
    [Range(1, 999, ErrorMessage = "Số lượng phải từ 1 đến 999.")]
    public int Quantity { get; set; }
}

public class SyncCartDto
{
    public List<SyncCartItemDto> Items { get; set; } = new();
}

public class SyncCartItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
    public List<int>? AddOnProductIds { get; set; }
}
