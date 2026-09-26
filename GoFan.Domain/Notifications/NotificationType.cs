namespace GoFan.Domain.Notifications;

public enum NotificationType
{
    OrderCreated,
    OrderConfirmed,
    OrderShipping,
    OrderCompleted,
    OrderCanceled,
    NewChatMessage,
    ProductPromotion,
    NewOrder,
    LowStock,
    OutOfStock,
    NewCustomerMessage
}