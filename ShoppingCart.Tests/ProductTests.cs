using ShoppingCart.Core;
using Xunit;

namespace ShoppingCart.Tests;

public class ProductTests
{
    [Fact]
    public void Product_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var product = new Product("Laptop", 1500.00m, "Electronics");

        // Assert
        Assert.Equal("Laptop", product.Name);
        Assert.Equal(1500.00m, product.Price);
        Assert.Equal("Electronics", product.Category);
    }

    [Theory]
    [InlineData("", 100, "Electronics")]
    [InlineData(null, 100, "Electronics")]
    [InlineData("   ", 100, "Electronics")]
    public void Product_WithEmptyName_ShouldThrowException(string name, decimal price, string category)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Product(name, price, category));
        Assert.Contains("name", exception.Message.ToLower());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100.50)]
    public void Product_WithNegativePrice_ShouldThrowException(decimal price)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Product("Laptop", price, "Electronics"));
        Assert.Contains("price", exception.Message.ToLower());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(0.01)]
    [InlineData(1000000)]
    public void Product_WithValidPriceBoundaries_ShouldCreateSuccessfully(decimal price)
    {
        // Arrange & Act
        var product = new Product("Test Product", price, "Test");

        // Assert
        Assert.Equal(price, product.Price);
    }

    [Theory]
    [InlineData("Product", 100, "")]
    [InlineData("Product", 100, null)]
    [InlineData("Product", 100, "   ")]
    public void Product_WithEmptyCategory_ShouldThrowException(string name, decimal price, string category)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Product(name, price, category));
        Assert.Contains("category", exception.Message.ToLower());
    }

    [Fact]
    public void Product_Equals_ShouldReturnTrueForSameProducts()
    {
        // Arrange
        var product1 = new Product("Laptop", 1500.00m, "Electronics");
        var product2 = new Product("Laptop", 1500.00m, "Electronics");

        // Act & Assert
        Assert.Equal(product1, product2);
    }

    [Fact]
    public void Product_Equals_ShouldReturnFalseForDifferentProducts()
    {
        // Arrange
        var product1 = new Product("Laptop", 1500.00m, "Electronics");
        var product2 = new Product("Phone", 800.00m, "Electronics");

        // Act & Assert
        Assert.NotEqual(product1, product2);
    }
}
