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