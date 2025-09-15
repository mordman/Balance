using SourceClearCodeTest.Application.Interfaces;
using SourceClearCodeTest.Core.Entities;
using SourceClearCodeTest.Core.Interfaces;

namespace SourceClearCodeTest.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _unitOfWork.Products.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _unitOfWork.Products.GetAllAsync();
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                return product;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Products.UpdateAsync(product);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product != null)
                {
                    await _unitOfWork.Products.DeleteAsync(product);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();
                }
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _unitOfWork.Products.GetProductsByPriceRangeAsync(minPrice, maxPrice);
        }

        public async Task UpdateProductQuantityAsync(int id, int quantity)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product != null)
                {
                    product.Quantity = quantity;
                    await _unitOfWork.Products.UpdateAsync(product);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();
                }
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}