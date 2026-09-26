using GoFan.Domain.Admins;
using GoFan.Domain.Carts;
using GoFan.Domain.Chat;
using GoFan.Domain.Combos;
using GoFan.Domain.Configurations;
using GoFan.Domain.Customers;
using GoFan.Domain.Favorites;
using GoFan.Domain.Inventory;
using GoFan.Domain.Notifications;
using GoFan.Domain.Orders;
using GoFan.Domain.Products;
using GoFan.Domain.Promotions;
using GoFan.Domain.Reviews;
using Microsoft.EntityFrameworkCore;

namespace GoFan.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerAddress> CustomerAddresses => Set<CustomerAddress>();

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductMedia> ProductMedias => Set<ProductMedia>();
    public DbSet<ProductAddOn> ProductAddOns => Set<ProductAddOn>();

    public DbSet<Promotion> Promotions => Set<Promotion>();
    public DbSet<ProductPromotion> ProductPromotions => Set<ProductPromotion>();

    public DbSet<Combo> Combos => Set<Combo>();
    public DbSet<ComboItem> ComboItems => Set<ComboItem>();

    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<CartItemAddOn> CartItemAddOns => Set<CartItemAddOn>();

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderItemAddOn> OrderItemAddOns => Set<OrderItemAddOn>();

    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<StockAdjustment> StockAdjustments => Set<StockAdjustment>();

    public DbSet<Favorite> Favorites => Set<Favorite>();

    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<ReviewMedia> ReviewMedias => Set<ReviewMedia>();

    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<MessageMedia> MessageMedias => Set<MessageMedia>();

    public DbSet<Notification> Notifications => Set<Notification>();

    public DbSet<SystemConfiguration> SystemConfigurations => Set<SystemConfiguration>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}