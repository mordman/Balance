using Microsoft.EntityFrameworkCore;
using SourceClearCodeTest.Core.Entities;
using SourceClearCodeTest.Core.Interfaces;
using SourceClearCodeTest.Infrastructure.Data;

namespace SourceClearCodeTest.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> AddAsync(Product entity)
        {
            await _context.Products.AddAsync(entity);
            return entity;
        }

        public async Task UpdateAsync(Product entity)
        {
            await Task.Run(() => _context.Entry(entity).State = EntityState.Modified);
        }

        public async Task DeleteAsync(Product entity)
        {
            await Task.Run(() => _context.Products.Remove(entity));
        }

        public async Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _context.Products
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .ToListAsync();
        }
    }
}