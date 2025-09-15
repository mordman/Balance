using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SourceClearCodeTest.Core.Entities;
using SourceClearCodeTest.Infrastructure.Data;

namespace SourceClearCodeTest.Tests;

[TestClass]
public class ProductRepositoryTests
{
    private ApplicationDbContext _context = null!;

    [TestInitialize]
    public async Task Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new ApplicationDbContext(options);
        await _context.Database.OpenConnectionAsync();
        await _context.Database.EnsureCreatedAsync();
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    [TestMethod]
    public async Task Database_ShouldBeSeeded_WithInitialData()
    {
        // Arrange & Act
        var products = await _context.Products.ToListAsync();

        // Assert
        Assert.AreEqual(3, products.Count);
        Assert.IsTrue(products.Any(p => p.Name == "Product 1"));
        Assert.IsTrue(products.Any(p => p.Name == "Product 2"));
        Assert.IsTrue(products.Any(p => p.Name == "Product 3"));
    }

    [TestMethod]
    public async Task AddProduct_ShouldIncreaseCount()
    {
        // Arrange
        var newProduct = new Product
        {
            Name = "Test Product",
            Price = 25.99m,
            Quantity = 30
        };

        // Act
        await _context.Products.AddAsync(newProduct);
        await _context.SaveChangesAsync();

        // Assert
        var products = await _context.Products.ToListAsync();
        Assert.AreEqual(4, products.Count);
        var addedProduct = await _context.Products.FirstOrDefaultAsync(p => p.Name == "Test Product");
        Assert.IsNotNull(addedProduct);
        Assert.AreEqual(25.99m, addedProduct.Price);
        Assert.AreEqual(30, addedProduct.Quantity);
    }

    [TestMethod]
    public async Task UpdateProduct_ShouldModifyEntity()
    {
        // Arrange
        var product = await _context.Products.FirstAsync();
        var originalName = product.Name;
        var newName = "Updated Product";

        // Act
        product.Name = newName;
        await _context.SaveChangesAsync();

        // Assert
        var updatedProduct = await _context.Products.FindAsync(product.Id);
        Assert.IsNotNull(updatedProduct);
        Assert.AreNotEqual(originalName, updatedProduct.Name);
        Assert.AreEqual(newName, updatedProduct.Name);
    }

    [TestMethod]
    public async Task DeleteProduct_ShouldDecreaseCount()
    {
        // Arrange
        var product = await _context.Products.FirstAsync();

        // Act
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        // Assert
        var products = await _context.Products.ToListAsync();
        Assert.AreEqual(2, products.Count);
        var deletedProduct = await _context.Products.FindAsync(product.Id);
        Assert.IsNull(deletedProduct);
    }
}