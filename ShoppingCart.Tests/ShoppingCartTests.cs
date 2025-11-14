using ShoppingCart.Core;
using Moq;
using Xunit;

namespace ShoppingCart.Tests;

public class ShoppingCartTests
{
    private readonly Mock<IDiscountService> _discountServiceMock;
    private readonly Core.ShoppingCart _cart;

    public ShoppingCartTests()
    {
        _discountServiceMock = new Mock<IDiscountService>();
        _discountServiceMock.Setup(s => s.ApplyDiscount(It.IsAny<Product>(), It.IsAny<decimal>()))
            .Returns<Product, decimal>((p, price) => price);
        
        _cart = new Core.ShoppingCart(_discountServiceMock.Object);
    }

    [Fact]
    public void AddProduct_WithValidProduct_ShouldAddToCart()
    {
        // Arrange
        var product = new Product("Laptop", 1500.00m, "Electronics");

        // Act
        _cart.AddProduct(product, 2);

        // Assert
        Assert.Single(_cart.Items);
        Assert.Equal(2, _cart.Items[0].Quantity);
    }

    [Fact]
    public void AddProduct_SameProductTwice_ShouldIncreaseQuantity()
    {
        // Arrange
        var product = new Product("Laptop", 1500.00m, "Electronics");

        // Act
        _cart.AddProduct(product, 2);
        _cart.AddProduct(product, 3);

        // Assert
        Assert.Single(_cart.Items);
        Assert.Equal(5, _cart.Items[0].Quantity);
    }

    [Fact]
    public void RemoveProduct_ShouldDecreaseQuantity()
    {
        // Arrange
        var product = new Product("Laptop", 1500.00m, "Electronics");
        _cart.AddProduct(product, 5);

        // Act
        _cart.RemoveProduct(product, 2);

        // Assert
        Assert.Equal(3, _cart.Items[0].Quantity);
    }

    [Fact]
    public void CalculateTotal_WithProducts_ShouldCalculateCorrectly()
    {
        // Arrange
        var product1 = new Product("Laptop", 1500.00m, "Electronics");
        var product2 = new Product("Book", 20.00m, "Books");
        
        _cart.AddProduct(product1, 2); // 3000
        _cart.AddProduct(product2, 3); // 60

        // Act
        var total = _cart.CalculateTotal();

        // Assert
        Assert.Equal(3060.00m, total);
    }

    [Fact]
    public void CalculateTotal_WithDiscount_ShouldApplyDiscount()
    {
        // Arrange
        var product = new Product("Laptop", 1000.00m, "Electronics");
        _discountServiceMock.Setup(s => s.ApplyDiscount(It.IsAny<Product>(), It.IsAny<decimal>()))
            .Returns<Product, decimal>((p, price) => price * 0.9m);
        
        _cart.AddProduct(product, 1);

        // Act
        var total = _cart.CalculateTotal();

        // Assert
        Assert.Equal(900.00m, total);
        _discountServiceMock.Verify(s => s.ApplyDiscount(It.IsAny<Product>(), It.IsAny<decimal>()), Times.Once);
    }

    [Theory]
    [InlineData(1, 100)]
    [InlineData(100, 10000)]
    public void CalculateTotal_BoundaryQuantities_ShouldCalculateCorrectly(int quantity, decimal expectedTotal)
    {
        // Arrange
        var product = new Product("Item", 100.00m, "Test");
        _cart.AddProduct(product, quantity);

        // Act
        var total = _cart.CalculateTotal();

        // Assert
        Assert.Equal(expectedTotal, total);
    }
}
