using GoFan.Application.Interfaces.Repositories;
using GoFan.Domain.Products;
using GoFan.Domain.Promotions;
using GoFan.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GoFan.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids)
        {
            var idList = ids.Distinct().ToList();
            if (!idList.Any())
            {
                return new List<Product>();
            }

            return await _context.Products
                .Where(p => idList.Contains(p.Id))
                .ToListAsync();
        }

        public async Task<Promotion?> GetActivePromotionAsync(
            int productId,
            DateTime now)
        {
            return await _context.ProductPromotions
                .Where(pp => pp.ProductId == productId)
                .Join(
                    _context.Promotions,
                    pp => pp.PromotionId,
                    promotion => promotion.Id,
                    (pp, promotion) => promotion)
                .Where(promotion =>
                    promotion.Active &&
                    promotion.StartAt <= now &&
                    promotion.EndAt >= now)
                .FirstOrDefaultAsync();
        }

        public async Task<Dictionary<int, Promotion>> GetActivePromotionsAsync(
            IEnumerable<int> productIds,
            DateTime now)
        {
            var idList = productIds.Distinct().ToList();
            if (!idList.Any())
            {
                return new Dictionary<int, Promotion>();
            }

            var list = await _context.ProductPromotions
                .Where(pp => idList.Contains(pp.ProductId))
                .Join(
                    _context.Promotions,
                    pp => pp.PromotionId,
                    promotion => promotion.Id,
                    (pp, promotion) => new { pp.ProductId, Promotion = promotion })
                .Where(x =>
                    x.Promotion.Active &&
                    x.Promotion.StartAt <= now &&
                    x.Promotion.EndAt >= now)
                .ToListAsync();

            var result = new Dictionary<int, Promotion>();
            foreach (var item in list)
            {
                if (!result.ContainsKey(item.ProductId) ||
                    result[item.ProductId].DiscountPercent < item.Promotion.DiscountPercent)
                {
                    result[item.ProductId] = item.Promotion;
                }
            }

            return result;
        }

        public async Task<List<ProductMedia>> GetMediasAsync(int productId)
        {
            return await _context.ProductMedias
                .Where(m => m.ProductId == productId)
                .AsNoTracking()
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        public async Task<List<ProductAddOn>> GetAddOnsAsync(int productId)
        {
            return await _context.ProductAddOns
                .Where(a => a.MainProductId == productId && a.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Product> AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}