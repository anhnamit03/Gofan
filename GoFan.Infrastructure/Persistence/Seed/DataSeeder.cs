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

namespace GoFan.Infrastructure.Persistence.Seed;

public static class DataSeeder
{
    public static async Task SeedAllAsync(AppDbContext context)
    {
        await EnsureSystemConfigurationsAsync(context);
        var admins = await EnsureAdminsAsync(context);
        var customers = await EnsureCustomersAndAddressesAsync(context);
        var (categories, products) = await EnsureCategoriesAndProductsAsync(context);
        var (promotions, combos) = await EnsurePromotionsAndCombosAsync(context, products);
        await EnsureInventoriesAsync(context, products, admins);
        await EnsureCartsAndFavoritesAsync(context, customers, products, combos);
        var orders = await EnsureOrdersAndReviewsAsync(context, customers, products, combos);
        await EnsureChatAndNotificationsAsync(context, customers, admins, products, orders);
    }

    public static async Task EnsureSystemConfigurationsAsync(AppDbContext context)
    {
        var configs = new List<SystemConfiguration>
        {
            new() { Key = "STORE_NAME", Value = "GoFan - Thế Giới Quạt Điện & Giải Pháp Làm Mát", Description = "Tên thương hiệu hệ thống", UpdatedAt = DateTime.UtcNow },
            new() { Key = "HOTLINE", Value = "1900 6868", Description = "Hotline tổng đài CSKH 24/7", UpdatedAt = DateTime.UtcNow },
            new() { Key = "SUPPORT_EMAIL", Value = "support@gofan.vn", Description = "Email hỗ trợ khách hàng", UpdatedAt = DateTime.UtcNow },
            new() { Key = "STORE_ADDRESS", Value = "123 Đường Cầu Giấy, Quận Cầu Giấy, Hà Nội", Description = "Địa chỉ showroom chính", UpdatedAt = DateTime.UtcNow },
            new() { Key = "FREE_SHIPPING_THRESHOLD", Value = "500000", Description = "Ngưỡng giá trị đơn hàng được miễn phí giao hàng (VNĐ)", UpdatedAt = DateTime.UtcNow },
            new() { Key = "WARRANTY_POLICY", Value = "Bảo hành chính hãng 24 tháng đối với động cơ quạt, đổi mới trong 30 ngày nếu phát hiện lỗi từ nhà sản xuất.", Description = "Chính sách bảo hành", UpdatedAt = DateTime.UtcNow }
        };

        foreach (var c in configs)
        {
            if (!await context.SystemConfigurations.AnyAsync(x => x.Key == c.Key))
            {
                await context.SystemConfigurations.AddAsync(c);
            }
        }

        await context.SaveChangesAsync();
    }

    public static async Task<List<Admin>> EnsureAdminsAsync(AppDbContext context)
    {
        var admins = new List<Admin>();

        var defaultPasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123456");

        var admin1 = await context.Admins.FirstOrDefaultAsync(a => a.Email == "admin@gofan.vn");
        if (admin1 == null)
        {
            admin1 = new Admin
            {
                Email = "admin@gofan.vn",
                FullName = "Quản trị viên hệ thống",
                PasswordHash = defaultPasswordHash,
                AvatarUrl = "https://images.unsplash.com/photo-1570295999919-56ceb5ecca61?auto=format&fit=crop&w=400&q=80",
                Active = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await context.Admins.AddAsync(admin1);
            await context.SaveChangesAsync();
        }
        admins.Add(admin1);

        var admin2 = await context.Admins.FirstOrDefaultAsync(a => a.Email == "manager@gofan.vn");
        if (admin2 == null)
        {
            admin2 = new Admin
            {
                Email = "manager@gofan.vn",
                FullName = "Nguyễn Văn Quản Lý (Kho & Vận hành)",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager@123456"),
                AvatarUrl = "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?auto=format&fit=crop&w=400&q=80",
                Active = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await context.Admins.AddAsync(admin2);
            await context.SaveChangesAsync();
        }
        admins.Add(admin2);

        return admins;
    }

    public static async Task<List<Customer>> EnsureCustomersAndAddressesAsync(AppDbContext context)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Customer@123456");
        var customers = new List<Customer>();

        var customerData = new[]
        {
            new
            {
                Phone = "0987654321",
                Email = "nguyenvana@gmail.com",
                FullName = "Nguyễn Văn An",
                Avatar = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&w=400&q=80",
                Password = "Customer@123456",
                Addresses = new[]
                {
                    new
                    {
                        Province = "Hà Nội",
                        District = "Quận Cầu Giấy",
                        Ward = "Phường Dịch Vọng Hậu",
                        Address = "Số 18, Ngõ 86 Phố Duy Tân",
                        Note = "Giao hàng tận nơi, gọi trước khi giao",
                        IsDefault = true
                    }
                }
            },
            new
            {
                Phone = "0912345678",
                Email = "tranmai@gmail.com",
                FullName = "Trần Thị Mai",
                Avatar = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=400&q=80",
                Password = "Customer@123456",
                Addresses = new[]
                {
                    new
                    {
                        Province = "Thành phố Hồ Chí Minh",
                        District = "Quận 1",
                        Ward = "Phường Bến Nghé",
                        Address = "Căn hộ 12B, Tòa tháp Landmark",
                        Note = "Giao giờ hành chính, gọi trước khi giao",
                        IsDefault = true
                    }
                }
            },
            new
            {
                Phone = "0909123456",
                Email = "lenam@gmail.com",
                FullName = "Lê Hoàng Nam",
                Avatar = "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&w=400&q=80",
                Password = "Customer@123456",
                Addresses = new[]
                {
                    new
                    {
                        Province = "Đà Nẵng",
                        District = "Quận Hải Châu",
                        Ward = "Phường Thạch Thang",
                        Address = "45 Đường Trần Phú",
                        Note = "Giao hàng tận nhà",
                        IsDefault = true
                    }
                }
            },
            new
            {
                Phone = "0911223344",
                Email = "customer@gmail.com",
                FullName = "Nguyễn Anh Nam",
                Avatar = "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?auto=format&fit=crop&w=400&q=80",
                Password = "anhnam0123",
                Addresses = new[]
                {
                    new
                    {
                        Province = "Hà Nội",
                        District = "Quận Cầu Giấy",
                        Ward = "Phường Dịch Vọng Hậu",
                        Address = "Số 102 Đường Cầu Giấy",
                        Note = "Nhà riêng - Giao hàng tận nơi, gọi trước khi giao",
                        IsDefault = true
                    },
                    new
                    {
                        Province = "Hà Nội",
                        District = "Quận Nam Từ Liêm",
                        Ward = "Phường Mễ Trì",
                        Address = "Tầng 12, Tòa nhà Keangnam Landmark 72, Đường Phạm Hùng",
                        Note = "Văn phòng công ty - Giờ hành chính (8h30 - 17h30), gửi lễ tân",
                        IsDefault = false
                    },
                    new
                    {
                        Province = "Hà Nội",
                        District = "Quận Đống Đa",
                        Ward = "Phường Ô Chợ Dừa",
                        Address = "Số 25 Ngõ 168 Đường Hào Nam",
                        Note = "Nhà bố mẹ - Giao buổi tối sau 18h hoặc cuối tuần, gọi trước 15 phút",
                        IsDefault = false
                    },
                    new
                    {
                        Province = "Thành phố Hồ Chí Minh",
                        District = "Quận 1",
                        Ward = "Phường Bến Nghé",
                        Address = "Căn hộ A2.15, Chung cư Vinhomes Golden River, Số 2 Tôn Đức Thắng",
                        Note = "Căn hộ chi nhánh - Bấm chuông căn hộ hoặc gửi sảnh ban quản lý",
                        IsDefault = false
                    }
                }
            },
            new
            {
                Phone = "0934567890",
                Email = "phamha@gmail.com",
                FullName = "Phạm Thu Hà",
                Avatar = "https://images.unsplash.com/photo-1494790108377-be9c29b29330?auto=format&fit=crop&w=400&q=80",
                Password = "Customer@123456",
                Addresses = new[]
                {
                    new
                    {
                        Province = "Cần Thơ",
                        District = "Quận Ninh Kiều",
                        Ward = "Phường An Khánh",
                        Address = "78 Đường Nguyễn Văn Cừ",
                        Note = "Giao hàng tận nơi",
                        IsDefault = true
                    }
                }
            }
        };

        foreach (var c in customerData)
        {
            var customer = await context.Customers.FirstOrDefaultAsync(x => x.Email == c.Email || x.Phone == c.Phone);
            var pwdHash = BCrypt.Net.BCrypt.HashPassword(c.Password);
            if (customer == null)
            {
                customer = new Customer
                {
                    Phone = c.Phone,
                    Email = c.Email,
                    FullName = c.FullName,
                    AvatarUrl = c.Avatar,
                    PasswordHash = pwdHash,
                    Active = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await context.Customers.AddAsync(customer);
                await context.SaveChangesAsync();
            }
            else
            {
                customer.FullName = c.FullName;
                customer.AvatarUrl = c.Avatar;
                customer.PasswordHash = pwdHash;
                customer.Active = true;
                customer.UpdatedAt = DateTime.UtcNow;
                await context.SaveChangesAsync();
            }

            foreach (var addrDef in c.Addresses)
            {
                var addrExists = await context.CustomerAddresses.AnyAsync(a =>
                    a.CustomerId == customer.Id &&
                    a.DetailedAddress == addrDef.Address);

                if (!addrExists)
                {
                    var address = new CustomerAddress
                    {
                        CustomerId = customer.Id,
                        Province = addrDef.Province,
                        District = addrDef.District,
                        Ward = addrDef.Ward,
                        DetailedAddress = addrDef.Address,
                        Note = addrDef.Note,
                        IsDefault = addrDef.IsDefault,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await context.CustomerAddresses.AddAsync(address);
                }
            }
            await context.SaveChangesAsync();
            customers.Add(customer);
        }

        return customers;
    }

    public static async Task<(List<Category> Categories, List<Product> Products)> EnsureCategoriesAndProductsAsync(AppDbContext context)
    {
        var categoryDefs = new[]
        {
            new { Name = "Quạt Đứng & Quạt Cây", Description = "Các dòng quạt cây cao cấp công suất lớn, độ êm ái cao cho phòng khách và phòng ngủ" },
            new { Name = "Quạt Trần Hiện Đại", Description = "Quạt trần trang trí, quạt trần có đèn LED và điều khiển thông minh tiết kiệm điện" },
            new { Name = "Quạt Treo Tường", Description = "Giải pháp làm mát tiết kiệm không gian cho văn phòng, phòng ăn và gia đình" },
            new { Name = "Quạt Bàn & Quạt Mini", Description = "Quạt bàn nhỏ gọn, quạt sạc USB để bàn văn phòng và phòng ngủ cá nhân" },
            new { Name = "Quạt Thông Gió & Hút Mùi", Description = "Quạt hút gắn tường, gắn trần nhà vệ sinh, nhà bếp thông gió hiệu quả" },
            new { Name = "Phụ Kiện & Bán Kèm", Description = "Điều khiển từ xa, cánh quạt thay thế, lồng bảo vệ an toàn cho trẻ nhỏ" }
        };

        var categories = new List<Category>();
        foreach (var def in categoryDefs)
        {
            var cat = await context.Categories.FirstOrDefaultAsync(c => c.Name == def.Name);
            if (cat == null)
            {
                cat = new Category
                {
                    Name = def.Name,
                    Description = def.Description,
                    Active = true,
                    CreateAt = DateTime.UtcNow,
                    UpdateAt = DateTime.UtcNow
                };
                await context.Categories.AddAsync(cat);
                await context.SaveChangesAsync();
            }
            categories.Add(cat);
        }

        var catStand = categories.First(c => c.Name.Contains("Đứng"));
        var catCeiling = categories.First(c => c.Name.Contains("Trần"));
        var catWall = categories.First(c => c.Name.Contains("Treo"));
        var catDesk = categories.First(c => c.Name.Contains("Bàn"));
        var catExhaust = categories.First(c => c.Name.Contains("Thông Gió"));
        var catAccessory = categories.First(c => c.Name.Contains("Phụ Kiện"));

        var productDefs = new[]
        {
            new
            {
                SKU = "GF-QD01",
                Name = "Quạt đứng GoFan Silent Wind Pro",
                CategoryId = catStand.Id,
                BasePrice = 1250000m,
                ImageUrl = "https://images.unsplash.com/photo-1618941716939-553df3c6c278?auto=format&fit=crop&w=800&q=80",
                Description = "Quạt đứng cao cấp tích hợp động cơ DC Inverter không chổi than, vận hành siêu êm ái với 12 cấp độ gió tinh chỉnh.",
                TechnicalInfo = "Công suất: 50W | Động cơ: DC Inverter | Sải cánh: 40cm (7 cánh) | Lưu lượng gió: 85 m³/phút | Cấp độ gió: 12 cấp | Hẹn giờ: 8 tiếng | Bảo hành: 24 tháng",
                Gallery = new[]
                {
                    "https://images.unsplash.com/photo-1618941716939-553df3c6c278?auto=format&fit=crop&w=800&q=80",
                    "https://images.unsplash.com/photo-1585771724684-38269d6639fd?auto=format&fit=crop&w=800&q=80",
                    "https://images.unsplash.com/photo-1513694203232-719a280e022f?auto=format&fit=crop&w=800&q=80"
                }
            },
            new
            {
                SKU = "GF-QT01",
                Name = "Quạt trần 5 cánh đèn LED GoFan Luxury Air",
                CategoryId = catCeiling.Id,
                BasePrice = 3450000m,
                ImageUrl = "https://images.unsplash.com/photo-1594913785162-e678a0c23dd9?auto=format&fit=crop&w=800&q=80",
                Description = "Quạt trần phong cách Bắc Âu hiện đại, tích hợp đèn LED 3 chế độ ánh sáng cùng remote điều khiển từ xa thông minh.",
                TechnicalInfo = "Công suất: 65W | Động cơ: Lõi đồng nguyên chất | Đèn LED: 24W (3 chế độ màu) | Sải cánh: 142cm | Điều khiển: Remote 6 cấp độ gió | Bảo hành: 36 tháng",
                Gallery = new[]
                {
                    "https://images.unsplash.com/photo-1594913785162-e678a0c23dd9?auto=format&fit=crop&w=800&q=80",
                    "https://images.unsplash.com/photo-1583847268964-b28dc8f51f92?auto=format&fit=crop&w=800&q=80"
                }
            },
            new
            {
                SKU = "GF-QTT01",
                Name = "Quạt treo tường có điều khiển GoFan WallMaster",
                CategoryId = catWall.Id,
                BasePrice = 790000m,
                ImageUrl = "https://images.unsplash.com/photo-1585771724684-38269d6639fd?auto=format&fit=crop&w=800&q=80",
                Description = "Quạt treo tường tiết kiệm không gian tối đa, động cơ bền bỉ, chuyển hướng đảo chiều điện tử tiện lợi.",
                TechnicalInfo = "Công suất: 55W | Sải cánh: 40cm | Lưu lượng gió: 72 m³/phút | Cấp độ gió: 3 cấp | Hẹn giờ: 6 tiếng | Bảo hành: 18 tháng",
                Gallery = new[]
                {
                    "https://images.unsplash.com/photo-1585771724684-38269d6639fd?auto=format&fit=crop&w=800&q=80"
                }
            },
            new
            {
                SKU = "GF-QB01",
                Name = "Quạt sạc mini để bàn xoay 120° GoFan DeskBreeze",
                CategoryId = catDesk.Id,
                BasePrice = 320000m,
                ImageUrl = "https://images.unsplash.com/photo-1544716278-ca5e3f4abd8c?auto=format&fit=crop&w=800&q=80",
                Description = "Quạt tích điện để bàn cổng Type-C, pin 4000mAh dùng 8-10 tiếng, xoay tự động 120 độ, nhỏ gọn cho dân văn phòng.",
                TechnicalInfo = "Công suất: 10W | Dung lượng pin: 4000mAh | Cổng sạc: USB Type-C | Trọng lượng: 450g | 4 cấp độ gió tự nhiên | Bảo hành: 12 tháng",
                Gallery = new[]
                {
                    "https://images.unsplash.com/photo-1544716278-ca5e3f4abd8c?auto=format&fit=crop&w=800&q=80"
                }
            },
            new
            {
                SKU = "GF-QH01",
                Name = "Quạt hút thông gió gắn trần GoFan AirVent 200",
                CategoryId = catExhaust.Id,
                BasePrice = 580000m,
                ImageUrl = "https://images.unsplash.com/photo-1581092160607-ee22621dd758?auto=format&fit=crop&w=800&q=80",
                Description = "Quạt hút mùi gắn trần siêu êm cho phòng tắm và bếp, tích hợp van ngăn mùi ngược và côn trùng xâm nhập.",
                TechnicalInfo = "Công suất: 30W | Kích thước khoét trần: 250x250mm | Lưu lượng hút: 240 m³/giờ | Độ ồn: < 38dB | Bảo hành: 24 tháng",
                Gallery = new[]
                {
                    "https://images.unsplash.com/photo-1581092160607-ee22621dd758?auto=format&fit=crop&w=800&q=80"
                }
            },
            new
            {
                SKU = "GF-PK-REMOTE",
                Name = "Điều khiển từ xa đa năng cho quạt GoFan",
                CategoryId = catAccessory.Id,
                BasePrice = 150000m,
                ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&w=800&q=80",
                Description = "Remote điều khiển quạt tần số 433MHz khoảng cách lên đến 15 mét, tương thích mọi dòng quạt thông minh GoFan.",
                TechnicalInfo = "Tần số: 433MHz | Khoảng cách bắt sóng: 15m | Pin: 2x AAA | Vật liệu: Nhựa ABS chống va đập",
                Gallery = new[]
                {
                    "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&w=800&q=80"
                }
            },
            new
            {
                SKU = "GF-PK-COVER",
                Name = "Lưới bọc lồng quạt an toàn cho trẻ em",
                CategoryId = catAccessory.Id,
                BasePrice = 60000m,
                ImageUrl = "https://images.unsplash.com/photo-1590794056226-79ef3a8147e1?auto=format&fit=crop&w=800&q=80",
                Description = "Lưới bọc chống kẹt tay cho trẻ nhỏ, chất liệu vải lưới dẻo dai thoáng khí không cản sức gió, chống bám bụi bẩn.",
                TechnicalInfo = "Chất liệu: Polyester dệt tổ ong | Đường kính co giãn: 40-45cm | Giặt sạch dễ dàng bằng máy giặt",
                Gallery = new[]
                {
                    "https://images.unsplash.com/photo-1590794056226-79ef3a8147e1?auto=format&fit=crop&w=800&q=80"
                }
            }
        };

        var products = new List<Product>();
        foreach (var def in productDefs)
        {
            var prod = await context.Products.FirstOrDefaultAsync(p => p.SKU == def.SKU);
            if (prod == null)
            {
                prod = new Product
                {
                    SKU = def.SKU,
                    Name = def.Name,
                    CategoryId = def.CategoryId,
                    BasePrice = def.BasePrice,
                    ImageUrl = def.ImageUrl,
                    Description = def.Description,
                    TechnicalInfo = def.TechnicalInfo,
                    Active = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await context.Products.AddAsync(prod);
                await context.SaveChangesAsync();

                int sortOrder = 1;
                foreach (var mediaUrl in def.Gallery)
                {
                    await context.ProductMedias.AddAsync(new ProductMedia
                    {
                        ProductId = prod.Id,
                        MediaType = MediaType.Image,
                        Url = mediaUrl,
                        AltText = $"{prod.Name} - Ảnh {sortOrder}",
                        SortOrder = sortOrder++,
                        CreatedAt = DateTime.UtcNow
                    });
                }
                await context.SaveChangesAsync();
            }
            products.Add(prod);
        }

        // Add-Ons Links
        var pStand = products.FirstOrDefault(p => p.SKU == "GF-QD01");
        var pCeiling = products.FirstOrDefault(p => p.SKU == "GF-QT01");
        var pRemote = products.FirstOrDefault(p => p.SKU == "GF-PK-REMOTE");
        var pCover = products.FirstOrDefault(p => p.SKU == "GF-PK-COVER");

        if (pStand != null && pRemote != null)
        {
            if (!await context.ProductAddOns.AnyAsync(a => a.MainProductId == pStand.Id && a.AddOnProductId == pRemote.Id))
            {
                await context.ProductAddOns.AddAsync(new ProductAddOn { MainProductId = pStand.Id, AddOnProductId = pRemote.Id, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
            }
        }

        if (pStand != null && pCover != null)
        {
            if (!await context.ProductAddOns.AnyAsync(a => a.MainProductId == pStand.Id && a.AddOnProductId == pCover.Id))
            {
                await context.ProductAddOns.AddAsync(new ProductAddOn { MainProductId = pStand.Id, AddOnProductId = pCover.Id, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
            }
        }

        if (pCeiling != null && pRemote != null)
        {
            if (!await context.ProductAddOns.AnyAsync(a => a.MainProductId == pCeiling.Id && a.AddOnProductId == pRemote.Id))
            {
                await context.ProductAddOns.AddAsync(new ProductAddOn { MainProductId = pCeiling.Id, AddOnProductId = pRemote.Id, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
            }
        }

        await context.SaveChangesAsync();

        return (categories, products);
    }

    public static async Task<(List<Promotion> Promotions, List<Combo> Combos)> EnsurePromotionsAndCombosAsync(AppDbContext context, List<Product> products)
    {
        var promotions = new List<Promotion>();

        var promo1 = await context.Promotions.FirstOrDefaultAsync(p => p.Name == "Đón Hè Rực Rỡ 2026");
        if (promo1 == null)
        {
            promo1 = new Promotion
            {
                Name = "Đón Hè Rực Rỡ 2026",
                DiscountPercent = 15m,
                StartAt = DateTime.UtcNow.AddDays(-5),
                EndAt = DateTime.UtcNow.AddDays(25),
                Active = true,
                CreatedAt = DateTime.UtcNow
            };
            await context.Promotions.AddAsync(promo1);
            await context.SaveChangesAsync();
        }
        promotions.Add(promo1);

        var promo2 = await context.Promotions.FirstOrDefaultAsync(p => p.Name == "Tuần Lễ Quạt Trần");
        if (promo2 == null)
        {
            promo2 = new Promotion
            {
                Name = "Tuần Lễ Quạt Trần",
                DiscountPercent = 10m,
                StartAt = DateTime.UtcNow.AddDays(-2),
                EndAt = DateTime.UtcNow.AddDays(12),
                Active = true,
                CreatedAt = DateTime.UtcNow
            };
            await context.Promotions.AddAsync(promo2);
            await context.SaveChangesAsync();
        }
        promotions.Add(promo2);

        // Link product promotions
        var pStand = products.FirstOrDefault(p => p.SKU == "GF-QD01");
        var pCeiling = products.FirstOrDefault(p => p.SKU == "GF-QT01");

        if (pStand != null && !await context.ProductPromotions.AnyAsync(pp => pp.ProductId == pStand.Id && pp.PromotionId == promo1.Id))
        {
            await context.ProductPromotions.AddAsync(new ProductPromotion { ProductId = pStand.Id, PromotionId = promo1.Id, CreatedAt = DateTime.UtcNow });
        }

        if (pCeiling != null && !await context.ProductPromotions.AnyAsync(pp => pp.ProductId == pCeiling.Id && pp.PromotionId == promo2.Id))
        {
            await context.ProductPromotions.AddAsync(new ProductPromotion { ProductId = pCeiling.Id, PromotionId = promo2.Id, CreatedAt = DateTime.UtcNow });
        }

        await context.SaveChangesAsync();

        // Combos
        var combos = new List<Combo>();

        var combo1 = await context.Combos.FirstOrDefaultAsync(c => c.Name == "Combo Giải Nhiệt Phòng Khách");
        if (combo1 == null)
        {
            combo1 = new Combo
            {
                Name = "Combo Giải Nhiệt Phòng Khách",
                Description = "Bộ đôi hoàn hảo gồm 1 Quạt đứng Silent Wind Pro + 1 Quạt bàn sạc pin DeskBreeze cho cả gia đình.",
                DiscountPercent = 12m,
                Active = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await context.Combos.AddAsync(combo1);
            await context.SaveChangesAsync();

            var pDesk = products.FirstOrDefault(p => p.SKU == "GF-QB01");
            if (pStand != null && pDesk != null)
            {
                await context.ComboItems.AddAsync(new ComboItem { ComboId = combo1.Id, ProductId = pStand.Id, CreatedAt = DateTime.UtcNow });
                await context.ComboItems.AddAsync(new ComboItem { ComboId = combo1.Id, ProductId = pDesk.Id, CreatedAt = DateTime.UtcNow });
                await context.SaveChangesAsync();
            }
        }
        combos.Add(combo1);

        var combo2 = await context.Combos.FirstOrDefaultAsync(c => c.Name == "Combo Căn Hộ Hiện Đại");
        if (combo2 == null)
        {
            combo2 = new Combo
            {
                Name = "Combo Căn Hộ Hiện Đại",
                Description = "Gói trang bị toàn diện gồm 1 Quạt trần cao cấp Luxury Air + 1 Quạt treo tường WallMaster tiết kiệm không gian.",
                DiscountPercent = 15m,
                Active = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await context.Combos.AddAsync(combo2);
            await context.SaveChangesAsync();

            var pWall = products.FirstOrDefault(p => p.SKU == "GF-QTT01");
            if (pCeiling != null && pWall != null)
            {
                await context.ComboItems.AddAsync(new ComboItem { ComboId = combo2.Id, ProductId = pCeiling.Id, CreatedAt = DateTime.UtcNow });
                await context.ComboItems.AddAsync(new ComboItem { ComboId = combo2.Id, ProductId = pWall.Id, CreatedAt = DateTime.UtcNow });
                await context.SaveChangesAsync();
            }
        }
        combos.Add(combo2);

        return (promotions, combos);
    }

    public static async Task EnsureInventoriesAsync(AppDbContext context, List<Product> products, List<Admin> admins)
    {
        var admin = admins.FirstOrDefault();
        if (admin == null) return;

        var quantities = new Dictionary<string, int>
        {
            ["GF-QD01"] = 65,
            ["GF-QT01"] = 28,
            ["GF-QTT01"] = 85,
            ["GF-QB01"] = 150,
            ["GF-QH01"] = 45,
            ["GF-PK-REMOTE"] = 250,
            ["GF-PK-COVER"] = 350
        };

        foreach (var p in products)
        {
            var qty = quantities.TryGetValue(p.SKU, out var q) ? q : 50;
            var inv = await context.Inventories.FirstOrDefaultAsync(i => i.ProductId == p.Id);
            if (inv == null)
            {
                inv = new Inventory
                {
                    ProductId = p.Id,
                    Quantity = qty,
                    UpdatedAt = DateTime.UtcNow
                };
                await context.Inventories.AddAsync(inv);
                await context.SaveChangesAsync();

                await context.StockAdjustments.AddAsync(new StockAdjustment
                {
                    InventoryId = inv.Id,
                    AdminId = admin.Id,
                    QuantityChange = qty,
                    Reason = "Nhập kho ban đầu từ nhà sản xuất",
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        await context.SaveChangesAsync();
    }

    public static async Task EnsureCartsAndFavoritesAsync(AppDbContext context, List<Customer> customers, List<Product> products, List<Combo> combos)
    {
        if (customers.Count == 0 || products.Count == 0) return;

        var c1 = customers[0];
        var c2 = customers.Count > 1 ? customers[1] : c1;

        var pStand = products.FirstOrDefault(p => p.SKU == "GF-QD01") ?? products[0];
        var pDesk = products.FirstOrDefault(p => p.SKU == "GF-QB01") ?? products[0];
        var pRemote = products.FirstOrDefault(p => p.SKU == "GF-PK-REMOTE") ?? products[0];

        // Cart 1
        var cart1 = await context.Carts.FirstOrDefaultAsync(c => c.CustomerId == c1.Id);
        if (cart1 == null)
        {
            cart1 = new Cart { CustomerId = c1.Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
            await context.Carts.AddAsync(cart1);
            await context.SaveChangesAsync();

            var item1 = new CartItem
            {
                CartId = cart1.Id,
                ProductId = pStand.Id,
                Quantity = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await context.CartItems.AddAsync(item1);
            await context.SaveChangesAsync();

            if (pRemote.Id != pStand.Id)
            {
                await context.CartItemAddOns.AddAsync(new CartItemAddOn
                {
                    CartItemId = item1.Id,
                    AddOnProductId = pRemote.Id,
                    Quantity = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
                await context.SaveChangesAsync();
            }
        }

        // Cart 2
        var cart2 = await context.Carts.FirstOrDefaultAsync(c => c.CustomerId == c2.Id);
        if (cart2 == null)
        {
            cart2 = new Cart { CustomerId = c2.Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
            await context.Carts.AddAsync(cart2);
            await context.SaveChangesAsync();

            await context.CartItems.AddAsync(new CartItem
            {
                CartId = cart2.Id,
                ProductId = pDesk.Id,
                Quantity = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }

        // Favorites
        if (!await context.Favorites.AnyAsync(f => f.CustomerId == c1.Id && f.ProductId == pStand.Id))
        {
            await context.Favorites.AddAsync(new Favorite { CustomerId = c1.Id, ProductId = pStand.Id, CreatedAt = DateTime.UtcNow });
        }

        if (products.Count > 1 && !await context.Favorites.AnyAsync(f => f.CustomerId == c2.Id && f.ProductId == products[1].Id))
        {
            await context.Favorites.AddAsync(new Favorite { CustomerId = c2.Id, ProductId = products[1].Id, CreatedAt = DateTime.UtcNow });
        }

        await context.SaveChangesAsync();
    }

    public static async Task<List<Order>> EnsureOrdersAndReviewsAsync(AppDbContext context, List<Customer> customers, List<Product> products, List<Combo> combos)
    {
        var orders = new List<Order>();
        if (customers.Count == 0 || products.Count == 0) return orders;

        var c1 = customers[0];
        var c2 = customers.Count > 1 ? customers[1] : c1;

        var pStand = products.FirstOrDefault(p => p.SKU == "GF-QD01") ?? products[0];
        var pCeiling = products.FirstOrDefault(p => p.SKU == "GF-QT01") ?? products[0];
        var pRemote = products.FirstOrDefault(p => p.SKU == "GF-PK-REMOTE");

        // Order 1 (Completed)
        var order1 = await context.Orders.FirstOrDefaultAsync(o => o.CustomerId == c1.Id && o.Status == OrderStatus.Completed);
        if (order1 == null)
        {
            order1 = new Order
            {
                CustomerId = c1.Id,
                Status = OrderStatus.Completed,
                TotalAmount = pStand.BasePrice + (pRemote != null ? pRemote.BasePrice : 0m),
                ShippingProvince = "Hà Nội",
                ShippingDistrict = "Quận Cầu Giấy",
                ShippingWard = "Phường Dịch Vọng Hậu",
                ShippingAddress = "Số 18, Ngõ 86 Phố Duy Tân",
                ShippingNote = "Giao buổi sáng",
                OrderNote = "Đóng gói cẩn thận giúp mình",
                PaymentMethod = PaymentMethod.COD,
                PaymentStatus = PaymentStatus.Paid,
                CreatedAt = DateTime.UtcNow.AddDays(-7),
                ConfirmedAt = DateTime.UtcNow.AddDays(-7).AddHours(1),
                ShippingAt = DateTime.UtcNow.AddDays(-6),
                CompletedAt = DateTime.UtcNow.AddDays(-5)
            };
            await context.Orders.AddAsync(order1);
            await context.SaveChangesAsync();

            var orderItem1 = new OrderItem
            {
                OrderId = order1.Id,
                ProductId = pStand.Id,
                ProductName = pStand.Name,
                SKU = pStand.SKU,
                Quantity = 1,
                UnitPrice = pStand.BasePrice,
                TotalPrice = pStand.BasePrice,
                CreatedAt = DateTime.UtcNow.AddDays(-7)
            };
            await context.OrderItems.AddAsync(orderItem1);
            await context.SaveChangesAsync();

            if (pRemote != null)
            {
                await context.OrderItemAddOns.AddAsync(new OrderItemAddOn
                {
                    OrderItemId = orderItem1.Id,
                    AddOnProductId = pRemote.Id,
                    AddOnProductName = pRemote.Name,
                    SKU = pRemote.SKU,
                    Quantity = 1,
                    UnitPrice = pRemote.BasePrice,
                    TotalPrice = pRemote.BasePrice,
                    CreatedAt = DateTime.UtcNow.AddDays(-7)
                });
                await context.SaveChangesAsync();
            }

            // Add Review for this completed order
            if (!await context.Reviews.AnyAsync(r => r.CustomerId == c1.Id && r.ProductId == pStand.Id && r.OrderId == order1.Id))
            {
                var review = new Review
                {
                    CustomerId = c1.Id,
                    ProductId = pStand.Id,
                    OrderId = order1.Id,
                    Rating = 5,
                    Content = "Quạt chạy cực kỳ êm ái, gió tự nhiên rất mát và dễ chịu. Dùng ban đêm trong phòng ngủ không hề nghe tiếng vo ve của động cơ. Đóng gói rất chắc chắn, giao hàng siêu nhanh!",
                    IsHidden = false,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-4)
                };
                await context.Reviews.AddAsync(review);
                await context.SaveChangesAsync();

                await context.ReviewMedias.AddAsync(new ReviewMedia
                {
                    ReviewId = review.Id,
                    MediaType = MediaType.Image,
                    Url = "https://images.unsplash.com/photo-1513694203232-719a280e022f?auto=format&fit=crop&w=800&q=80",
                    CreatedAt = DateTime.UtcNow.AddDays(-4)
                });
                await context.SaveChangesAsync();
            }
        }
        orders.Add(order1);

        // Order 2 (Shipping)
        var order2 = await context.Orders.FirstOrDefaultAsync(o => o.CustomerId == c2.Id && o.Status == OrderStatus.Shipping);
        if (order2 == null)
        {
            order2 = new Order
            {
                CustomerId = c2.Id,
                Status = OrderStatus.Shipping,
                TotalAmount = pCeiling.BasePrice,
                ShippingProvince = "Thành phố Hồ Chí Minh",
                ShippingDistrict = "Quận 1",
                ShippingWard = "Phường Bến Nghé",
                ShippingAddress = "Căn hộ 12B, Tòa tháp Landmark",
                ShippingNote = "Gọi trước khi giao 15 phút",
                OrderNote = "Giao hàng cẩn thận đồ điện tử",
                PaymentMethod = PaymentMethod.COD,
                PaymentStatus = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                ConfirmedAt = DateTime.UtcNow.AddDays(-1).AddHours(2),
                ShippingAt = DateTime.UtcNow.AddHours(-6)
            };
            await context.Orders.AddAsync(order2);
            await context.SaveChangesAsync();

            await context.OrderItems.AddAsync(new OrderItem
            {
                OrderId = order2.Id,
                ProductId = pCeiling.Id,
                ProductName = pCeiling.Name,
                SKU = pCeiling.SKU,
                Quantity = 1,
                UnitPrice = pCeiling.BasePrice,
                TotalPrice = pCeiling.BasePrice,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            });
            await context.SaveChangesAsync();
        }
        orders.Add(order2);

        return orders;
    }

    public static async Task EnsureChatAndNotificationsAsync(AppDbContext context, List<Customer> customers, List<Admin> admins, List<Product> products, List<Order> orders)
    {
        if (customers.Count == 0 || admins.Count == 0) return;

        var c1 = customers[0];
        var admin = admins[0];

        // Conversation
        var conversation = await context.Conversations.FirstOrDefaultAsync(cv => cv.CustomerId == c1.Id);
        if (conversation == null)
        {
            conversation = new Conversation
            {
                CustomerId = c1.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                UpdatedAt = DateTime.UtcNow.AddDays(-3).AddHours(1)
            };
            await context.Conversations.AddAsync(conversation);
            await context.SaveChangesAsync();

            // Customer asks
            var msg1 = new Message
            {
                ConversationId = conversation.Id,
                SenderId = c1.Id,
                Content = "Chào shop, quạt đứng Silent Wind Pro sải cánh bao nhiêu cm và dùng phòng 25m2 có mát không ạ?",
                IsRead = true,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                UpdatedAt = DateTime.UtcNow.AddDays(-3)
            };
            await context.Messages.AddAsync(msg1);

            // Admin answers
            var msg2 = new Message
            {
                ConversationId = conversation.Id,
                SenderId = admin.Id,
                Content = "Dạ chào anh An! Dòng quạt Silent Wind Pro có sải cánh 40cm với 7 cánh quạt khí động học, lưu lượng gió 85m³/phút rất thoải mái cho phòng từ 20-30m² anh nhé!",
                IsRead = true,
                CreatedAt = DateTime.UtcNow.AddDays(-3).AddMinutes(15),
                UpdatedAt = DateTime.UtcNow.AddDays(-3).AddMinutes(15)
            };
            await context.Messages.AddAsync(msg2);

            // Admin sends image
            var msg3 = new Message
            {
                ConversationId = conversation.Id,
                SenderId = admin.Id,
                Content = "Em gửi anh ảnh kích thước chi tiết và các cấp độ gió thực tế ạ.",
                IsRead = true,
                CreatedAt = DateTime.UtcNow.AddDays(-3).AddMinutes(16),
                UpdatedAt = DateTime.UtcNow.AddDays(-3).AddMinutes(16)
            };
            await context.Messages.AddAsync(msg3);
            await context.SaveChangesAsync();

            await context.MessageMedias.AddAsync(new MessageMedia
            {
                MessageId = msg3.Id,
                MediaType = MediaType.Image,
                Url = "https://images.unsplash.com/photo-1618941716939-553df3c6c278?auto=format&fit=crop&w=800&q=80",
                CreatedAt = DateTime.UtcNow.AddDays(-3).AddMinutes(16)
            });
            await context.SaveChangesAsync();
        }

        // Notifications
        var order1 = orders.FirstOrDefault();
        if (order1 != null)
        {
            if (!await context.Notifications.AnyAsync(n => n.RecipientId == c1.Id && n.Type == NotificationType.OrderCompleted))
            {
                await context.Notifications.AddAsync(new Notification
                {
                    RecipientId = c1.Id,
                    Type = NotificationType.OrderCompleted,
                    Title = "Đơn hàng đã giao thành công",
                    Content = $"Đơn hàng #{order1.Id} của quý khách đã được giao thành công. Cảm ơn bạn đã lựa chọn GoFan!",
                    IsRead = true,
                    ReferenceId = order1.Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                });
            }
        }

        if (!await context.Notifications.AnyAsync(n => n.RecipientId == c1.Id && n.Type == NotificationType.ProductPromotion))
        {
            await context.Notifications.AddAsync(new Notification
            {
                RecipientId = c1.Id,
                Type = NotificationType.ProductPromotion,
                Title = "Chương trình ưu đãi: Đón Hè Rực Rỡ 2026",
                Content = "Giảm ngay 15% cho dòng quạt đứng Silent Wind Pro cùng nhiều quà tặng hấp dẫn!",
                IsRead = false,
                ReferenceId = null,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            });
        }

        // Admin notification
        if (!await context.Notifications.AnyAsync(n => n.RecipientId == admin.Id && n.Type == NotificationType.NewOrder))
        {
            await context.Notifications.AddAsync(new Notification
            {
                RecipientId = admin.Id,
                Type = NotificationType.NewOrder,
                Title = "Có đơn hàng mới cần xử lý",
                Content = $"Khách hàng {c1.FullName} vừa đặt đơn hàng mới cần xác nhận.",
                IsRead = false,
                ReferenceId = order1?.Id,
                CreatedAt = DateTime.UtcNow.AddHours(-3)
            });
        }

        await context.SaveChangesAsync();
    }
}
