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
    [InlineData("Laptop", -1, "Electronics")]
    [InlineData("Laptop", 100, "")]
    public void Product_WithInvalidData_ShouldThrowException(string name, decimal price, string category)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Product(name, price, category));
    }

    [Theory]
    [InlineData(0, 0.01, 999999.99)] // Teste de graniță pentru prețuri
    public void Product_WithBoundaryPrices_ShouldWork(decimal price1, decimal price2, decimal price3)
    {
        // Act
        var product1 = new Product("Test1", price1, "Test");
        var product2 = new Product("Test2", price2, "Test");
        var product3 = new Product("Test3", price3, "Test");

        // Assert
        Assert.Equal(price1, product1.Price);
        Assert.Equal(price2, product2.Price);
        Assert.Equal(price3, product3.Price);
    }

    [Fact]
    public void Product_Equals_ShouldWorkCorrectly()
    {
        // Arrange
        var product1 = new Product("Laptop", 1500.00m, "Electronics");
        var product2 = new Product("Laptop", 1500.00m, "Electronics");
        var product3 = new Product("Phone", 800.00m, "Electronics");

        // Assert
        Assert.Equal(product1, product2);
        Assert.NotEqual(product1, product3);
    }
}
