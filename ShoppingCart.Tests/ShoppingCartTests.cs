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
        // Default: no discount applied
        _discountServiceMock.Setup(s => s.ApplyDiscount(It.IsAny<Product>(), It.IsAny<decimal>()))
            .Returns<Product, decimal>((p, price) => price);
        
        _cart = new Core.ShoppingCart(_discountServiceMock.Object);
    }

    [Fact]
    public void ShoppingCart_Constructor_WithNullDiscountService_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Core.ShoppingCart(null!));
    }

    [Fact]
    public void AddProduct_WithValidProduct_ShouldAddToCart()
    {
        // Arrange
        var product = new Product("Laptop", 1500.00m, "Electronics");

        // Act
        _cart.AddProduct(product);

        // Assert
        Assert.Single(_cart.Items);
        Assert.Equal(product, _cart.Items[0].Product);
        Assert.Equal(1, _cart.Items[0].Quantity);
    }

    [Fact]
    public void AddProduct_WithQuantity_ShouldAddCorrectQuantity()
    {
        // Arrange
        var product = new Product("Book", 20.00m, "Books");

        // Act
        _cart.AddProduct(product, 5);

        // Assert
        Assert.Single(_cart.Items);
        Assert.Equal(5, _cart.Items[0].Quantity);
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
    public void AddProduct_WithNullProduct_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _cart.AddProduct(null!));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void AddProduct_WithInvalidQuantity_ShouldThrowException(int quantity)
    {
        // Arrange
        var product = new Product("Laptop", 1500.00m, "Electronics");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _cart.AddProduct(product, quantity));
        Assert.Contains("quantity", exception.Message.ToLower());
    }

    [Fact]
    public void RemoveProduct_WithExistingProduct_ShouldDecreaseQuantity()
    {
        // Arrange
        var product = new Product("Laptop", 1500.00m, "Electronics");
        _cart.AddProduct(product, 5);

        // Act
        _cart.RemoveProduct(product, 2);

        // Assert
        Assert.Single(_cart.Items);
        Assert.Equal(3, _cart.Items[0].Quantity);
    }

    [Fact]
    public void RemoveProduct_WithExactQuantity_ShouldRemoveFromCart()
    {
        // Arrange
        var product = new Product("Laptop", 1500.00m, "Electronics");
        _cart.AddProduct(product, 3);

        // Act
        _cart.RemoveProduct(product, 3);

        // Assert
        Assert.Empty(_cart.Items);
    }

    [Fact]
    public void RemoveProduct_WithMoreThanAvailable_ShouldRemoveFromCart()
    {
        // Arrange
        var product = new Product("Laptop", 1500.00m, "Electronics");
        _cart.AddProduct(product, 3);

        // Act
        _cart.RemoveProduct(product, 5);

        // Assert
        Assert.Empty(_cart.Items);
    }

    [Fact]
    public void RemoveProduct_NonExistingProduct_ShouldThrowException()
    {
        // Arrange
        var product = new Product("Laptop", 1500.00m, "Electronics");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _cart.RemoveProduct(product));
    }

    [Fact]
    public void RemoveProduct_WithNullProduct_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _cart.RemoveProduct(null!));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RemoveProduct_WithInvalidQuantity_ShouldThrowException(int quantity)
    {
        // Arrange
        var product = new Product("Laptop", 1500.00m, "Electronics");
        _cart.AddProduct(product);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _cart.RemoveProduct(product, quantity));
        Assert.Contains("quantity", exception.Message.ToLower());
    }

    [Fact]
    public void Clear_ShouldRemoveAllItems()
    {
        // Arrange
        _cart.AddProduct(new Product("Laptop", 1500.00m, "Electronics"));
        _cart.AddProduct(new Product("Book", 20.00m, "Books"));

        // Act
        _cart.Clear();

        // Assert
        Assert.Empty(_cart.Items);
    }

    [Fact]
    public void CalculateTotal_EmptyCart_ShouldReturnZero()
    {
        // Act
        var total = _cart.CalculateTotal();

        // Assert
        Assert.Equal(0, total);
    }

    [Fact]
    public void CalculateTotal_WithProducts_ShouldCalculateCorrectTotal()
    {
        // Arrange
        var product1 = new Product("Laptop", 1500.00m, "Electronics");
        var product2 = new Product("Book", 20.00m, "Books");
        
        _cart.AddProduct(product1, 2); // 2 * 1500 = 3000
        _cart.AddProduct(product2, 3); // 3 * 20 = 60

        // Act
        var total = _cart.CalculateTotal();

        // Assert
        Assert.Equal(3060.00m, total);
    }

    [Fact]
    public void CalculateTotal_ShouldCallDiscountService()
    {
        // Arrange
        var product = new Product("Laptop", 1500.00m, "Electronics");
        _cart.AddProduct(product);

        // Act
        _cart.CalculateTotal();

        // Assert
        _discountServiceMock.Verify(
            s => s.ApplyDiscount(It.IsAny<Product>(), It.IsAny<decimal>()), 
            Times.Once);
    }

    [Fact]
    public void CalculateTotal_WithDiscount_ShouldApplyDiscount()
    {
        // Arrange
        var product = new Product("Laptop", 1000.00m, "Electronics");
        
        // Setup: 10% discount (return 90% of price)
        _discountServiceMock.Setup(s => s.ApplyDiscount(It.IsAny<Product>(), It.IsAny<decimal>()))
            .Returns<Product, decimal>((p, price) => price * 0.9m);
        
        _cart.AddProduct(product, 1);

        // Act
        var total = _cart.CalculateTotal();

        // Assert
        Assert.Equal(900.00m, total);
    }

    [Theory]
    [InlineData(1, 100)]
    [InlineData(10, 1000)]
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

    [Fact]
    public void GetTotalItems_EmptyCart_ShouldReturnZero()
    {
        // Act
        var totalItems = _cart.GetTotalItems();

        // Assert
        Assert.Equal(0, totalItems);
    }

    [Fact]
    public void GetTotalItems_WithMultipleProducts_ShouldReturnCorrectCount()
    {
        // Arrange
        _cart.AddProduct(new Product("Laptop", 1500.00m, "Electronics"), 2);
        _cart.AddProduct(new Product("Book", 20.00m, "Books"), 5);
        _cart.AddProduct(new Product("Shirt", 30.00m, "Clothing"), 3);

        // Act
        var totalItems = _cart.GetTotalItems();

        // Assert
        Assert.Equal(10, totalItems);
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(999999.99)]
    public void CalculateTotal_BoundaryPrices_ShouldCalculateCorrectly(decimal price)
    {
        // Arrange
        var product = new Product("Test", price, "Test");
        _cart.AddProduct(product);

        // Act
        var total = _cart.CalculateTotal();

        // Assert
        Assert.Equal(price, total);
    }
}
