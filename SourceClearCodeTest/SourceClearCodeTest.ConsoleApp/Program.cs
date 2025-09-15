using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SourceClearCodeTest.Application.Interfaces;
using SourceClearCodeTest.Application.Services;
using SourceClearCodeTest.Core.Entities;
using SourceClearCodeTest.Core.Interfaces;
using SourceClearCodeTest.Infrastructure.Data;
using SourceClearCodeTest.Infrastructure.Repositories;

var services = new ServiceCollection();

// Add database context
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=products.db"));

// Register dependencies
services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped<IProductService, ProductService>();

var serviceProvider = services.BuildServiceProvider();

// Initialize database
using (var scope = serviceProvider.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

var productService = serviceProvider.GetRequiredService<IProductService>();

while (true)
{
    Console.WriteLine("\nProduct Management System");
    Console.WriteLine("1. List all products");
    Console.WriteLine("2. Add new product");
    Console.WriteLine("3. Update product");
    Console.WriteLine("4. Delete product");
    Console.WriteLine("5. Search products by price range");
    Console.WriteLine("6. Update product quantity");
    Console.WriteLine("0. Exit");
    Console.Write("\nSelect an option: ");

    if (!int.TryParse(Console.ReadLine(), out int choice))
    {
        Console.WriteLine("Invalid input. Please try again.");
        continue;
    }

    try
    {
        switch (choice)
        {
            case 1:
                var products = await productService.GetAllProductsAsync();
                Console.WriteLine("\nAll Products:");
                foreach (var product in products)
                {
                    Console.WriteLine($"Id: {product.Id}, Name: {product.Name}, Price: ${product.Price}, Quantity: {product.Quantity}");
                }
                break;

            case 2:
                Console.Write("Enter product name: ");
                var name = Console.ReadLine();
                Console.Write("Enter price: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal price))
                {
                    Console.WriteLine("Invalid price format.");
                    break;
                }
                Console.Write("Enter quantity: ");
                if (!int.TryParse(Console.ReadLine(), out int quantity))
                {
                    Console.WriteLine("Invalid quantity format.");
                    break;
                }

                var newProduct = new Product { Name = name!, Price = price, Quantity = quantity };
                await productService.CreateProductAsync(newProduct);
                Console.WriteLine("Product added successfully!");
                break;

            case 3:
                Console.Write("Enter product ID to update: ");
                if (!int.TryParse(Console.ReadLine(), out int updateId))
                {
                    Console.WriteLine("Invalid ID format.");
                    break;
                }

                var existingProduct = await productService.GetProductByIdAsync(updateId);
                if (existingProduct == null)
                {
                    Console.WriteLine("Product not found.");
                    break;
                }

                Console.Write($"Enter new name (current: {existingProduct.Name}): ");
                var newName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newName))
                    existingProduct.Name = newName;

                Console.Write($"Enter new price (current: ${existingProduct.Price}): ");
                var priceInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(priceInput) && decimal.TryParse(priceInput, out decimal newPrice))
                    existingProduct.Price = newPrice;

                Console.Write($"Enter new quantity (current: {existingProduct.Quantity}): ");
                var quantityInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(quantityInput) && int.TryParse(quantityInput, out int newQuantity))
                    existingProduct.Quantity = newQuantity;

                await productService.UpdateProductAsync(existingProduct);
                Console.WriteLine("Product updated successfully!");
                break;

            case 4:
                Console.Write("Enter product ID to delete: ");
                if (!int.TryParse(Console.ReadLine(), out int deleteId))
                {
                    Console.WriteLine("Invalid ID format.");
                    break;
                }

                await productService.DeleteProductAsync(deleteId);
                Console.WriteLine("Product deleted successfully!");
                break;

            case 5:
                Console.Write("Enter minimum price: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal minPrice))
                {
                    Console.WriteLine("Invalid price format.");
                    break;
                }

                Console.Write("Enter maximum price: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal maxPrice))
                {
                    Console.WriteLine("Invalid price format.");
                    break;
                }

                var filteredProducts = await productService.GetProductsByPriceRangeAsync(minPrice, maxPrice);
                Console.WriteLine($"\nProducts between ${minPrice} and ${maxPrice}:");
                foreach (var product in filteredProducts)
                {
                    Console.WriteLine($"Id: {product.Id}, Name: {product.Name}, Price: ${product.Price}, Quantity: {product.Quantity}");
                }
                break;

            case 6:
                Console.Write("Enter product ID to update quantity: ");
                if (!int.TryParse(Console.ReadLine(), out int quantityUpdateId))
                {
                    Console.WriteLine("Invalid ID format.");
                    break;
                }

                Console.Write("Enter new quantity: ");
                if (!int.TryParse(Console.ReadLine(), out int updatedQuantity))
                {
                    Console.WriteLine("Invalid quantity format.");
                    break;
                }

                await productService.UpdateProductQuantityAsync(quantityUpdateId, updatedQuantity);
                Console.WriteLine("Product quantity updated successfully!");
                break;

            case 0:
                return;

            default:
                Console.WriteLine("Invalid option. Please try again.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}
