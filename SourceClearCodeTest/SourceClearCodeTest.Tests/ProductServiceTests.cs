using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SourceClearCodeTest.Core.Entities;
using SourceClearCodeTest.Core.Interfaces;
using SourceClearCodeTest.Application.Services;

namespace SourceClearCodeTest.Tests;

[TestClass]
public class ProductServiceTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock = null!;
    private Mock<IProductRepository> _productRepositoryMock = null!;
    private ProductService _productService = null!;

    [TestInitialize]
    public void Setup()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(uow => uow.Products).Returns(_productRepositoryMock.Object);
        _productService = new ProductService(_unitOfWorkMock.Object);
    }

    [TestMethod]
    public async Task GetProductByIdAsync_ExistingId_ReturnsProduct()
    {
        // Arrange
        var expectedProduct = new Product { Id = 1, Name = "Test Product", Price = 10.99m, Quantity = 100 };
        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(expectedProduct);

        // Act
        var result = await _productService.GetProductByIdAsync(1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedProduct.Id, result.Id);
        Assert.AreEqual(expectedProduct.Name, result.Name);
        Assert.AreEqual(expectedProduct.Price, result.Price);
        Assert.AreEqual(expectedProduct.Quantity, result.Quantity);
    }

    [TestMethod]
    public async Task CreateProductAsync_ValidProduct_ReturnsCreatedProduct()
    {
        // Arrange
        var product = new Product { Name = "New Product", Price = 15.99m, Quantity = 50 };

        _productRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);
        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync()).ReturnsAsync(true);

        // Act
        var result = await _productService.CreateProductAsync(product);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(product.Name, result.Name);
        Assert.AreEqual(product.Price, result.Price);
        Assert.AreEqual(product.Quantity, result.Quantity);

        _productRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.BeginTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CommitTransactionAsync(), Times.Once);
    }

    [TestMethod]
    public async Task CreateProductAsync_FailedSave_ThrowsException()
    {
        // Arrange
        var product = new Product { Name = "New Product", Price = 15.99m, Quantity = 50 };

        _productRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);
        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync()).ThrowsAsync(new Exception("Save failed"));

        // Act & Assert
        await Assert.ThrowsExceptionAsync<Exception>(() => _productService.CreateProductAsync(product));

        _unitOfWorkMock.Verify(uow => uow.RollbackTransactionAsync(), Times.Once);
    }

    [TestMethod]
    public async Task GetProductsByPriceRangeAsync_ValidRange_ReturnsFilteredProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1", Price = 10.99m },
            new Product { Id = 2, Name = "Product 2", Price = 20.50m }
        };

        _productRepositoryMock.Setup(repo => repo.GetProductsByPriceRangeAsync(10m, 25m))
            .ReturnsAsync(products);

        // Act
        var result = await _productService.GetProductsByPriceRangeAsync(10m, 25m);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
        CollectionAssert.AreEqual(products.Select(p => p.Id).ToList(), result.Select(p => p.Id).ToList());
    }

    [TestMethod]
    public async Task UpdateProductQuantityAsync_ValidProduct_UpdatesQuantity()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test Product", Quantity = 10 };
        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(product);

        // Act
        await _productService.UpdateProductQuantityAsync(1, 20);

        // Assert
        Assert.AreEqual(20, product.Quantity);
        _productRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }
}