using ShoppingCart.Core;
using Moq;
using Xunit;

namespace ShoppingCart.Tests;

public class DiscountServiceTests
{
    [Fact]
    public void ApplyDiscount_Electronics_ShouldApply10PercentDiscount()
    {
        // Arrange
        var service = new DiscountService();
        var product = new Product("Laptop", 1000.00m, "Electronics");

        // Act
        var discountedPrice = service.ApplyDiscount(product, 1000.00m);

        // Assert
        Assert.Equal(900.00m, discountedPrice); // 10% discount
    }

    [Fact]
    public void ApplyDiscount_Clothing_ShouldApply15PercentDiscount()
    {
        // Arrange
        var service = new DiscountService();
        var product = new Product("Shirt", 100.00m, "Clothing");

        // Act
        var discountedPrice = service.ApplyDiscount(product, 100.00m);

        // Assert
        Assert.Equal(85.00m, discountedPrice); // 15% discount
    }

    [Fact]
    public void ApplyDiscount_Books_ShouldApply5PercentDiscount()
    {
        // Arrange
        var service = new DiscountService();
        var product = new Product("Programming Book", 50.00m, "Books");

        // Act
        var discountedPrice = service.ApplyDiscount(product, 50.00m);

        // Assert
        Assert.Equal(47.50m, discountedPrice); // 5% discount
    }

    [Fact]
    public void ApplyDiscount_Food_ShouldApplyNoDiscount()
    {
        // Arrange
        var service = new DiscountService();
        var product = new Product("Apple", 5.00m, "Food");

        // Act
        var discountedPrice = service.ApplyDiscount(product, 5.00m);

        // Assert
        Assert.Equal(5.00m, discountedPrice); // No discount
    }

    [Fact]
    public void ApplyDiscount_UnknownCategory_ShouldApplyNoDiscount()
    {
        // Arrange
        var service = new DiscountService();
        var product = new Product("Mystery Item", 100.00m, "Unknown");

        // Act
        var discountedPrice = service.ApplyDiscount(product, 100.00m);

        // Assert
        Assert.Equal(100.00m, discountedPrice); // No discount for unknown category
    }

    [Theory]
    [InlineData(0.01, 0.009)]      // Small price
    [InlineData(10000, 9000)]      // Large price
    [InlineData(999.99, 899.991)]  // Decimal precision
    public void ApplyDiscount_ElectronicsBoundaryPrices_ShouldApplyCorrectDiscount(decimal price, decimal expected)
    {
        // Arrange
        var service = new DiscountService();
        var product = new Product("Electronic", price, "Electronics");

        // Act
        var discountedPrice = service.ApplyDiscount(product, price);

        // Assert
        Assert.Equal(expected, discountedPrice);
    }

    [Fact]
    public void ApplyDiscount_CaseInsensitiveCategory_ShouldApplyDiscount()
    {
        // Arrange
        var service = new DiscountService();
        var product1 = new Product("Item1", 100.00m, "electronics");
        var product2 = new Product("Item2", 100.00m, "ELECTRONICS");
        var product3 = new Product("Item3", 100.00m, "ElEcTrOnIcS");

        // Act
        var price1 = service.ApplyDiscount(product1, 100.00m);
        var price2 = service.ApplyDiscount(product2, 100.00m);
        var price3 = service.ApplyDiscount(product3, 100.00m);

        // Assert
        Assert.Equal(90.00m, price1);
        Assert.Equal(90.00m, price2);
        Assert.Equal(90.00m, price3);
    }

    [Fact]
    public void SetCategoryDiscount_ValidRate_ShouldUpdateDiscount()
    {
        // Arrange
        var service = new DiscountService();
        var product = new Product("Item", 100.00m, "NewCategory");

        // Act
        service.SetCategoryDiscount("NewCategory", 0.25m); // 25% discount
        var discountedPrice = service.ApplyDiscount(product, 100.00m);

        // Assert
        Assert.Equal(75.00m, discountedPrice);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    [InlineData(-1)]
    [InlineData(2)]
    public void SetCategoryDiscount_InvalidRate_ShouldThrowException(decimal rate)
    {
        // Arrange
        var service = new DiscountService();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => service.SetCategoryDiscount("Test", rate));
        Assert.Contains("discount rate", exception.Message.ToLower());
    }

    [Theory]
    [InlineData(0)]    // No discount
    [InlineData(1)]    // 100% discount
    [InlineData(0.5)]  // 50% discount
    public void SetCategoryDiscount_BoundaryRates_ShouldWork(decimal rate)
    {
        // Arrange
        var service = new DiscountService();
        var product = new Product("Item", 100.00m, "Test");

        // Act
        service.SetCategoryDiscount("Test", rate);
        var discountedPrice = service.ApplyDiscount(product, 100.00m);

        // Assert
        Assert.Equal(100.00m * (1 - rate), discountedPrice);
    }

    [Fact]
    public void ApplyDiscount_CalledMultipleTimes_WithSpy()
    {
        // Arrange - Create a spy using Moq
        var mockService = new Mock<DiscountService>();
        mockService.CallBase = true; // This makes it a spy - calls real implementation
        
        var product1 = new Product("Laptop", 1000.00m, "Electronics");
        var product2 = new Product("Book", 50.00m, "Books");
        var product3 = new Product("Shirt", 100.00m, "Clothing");

        // Act
        mockService.Object.ApplyDiscount(product1, 1000.00m);
        mockService.Object.ApplyDiscount(product2, 50.00m);
        mockService.Object.ApplyDiscount(product3, 100.00m);

        // Assert - Verify the spy was called 3 times
        mockService.Verify(s => s.ApplyDiscount(It.IsAny<Product>(), It.IsAny<decimal>()), Times.Exactly(3));
    }

    [Fact]
    public void ApplyDiscount_VerifyCalledWithSpecificProduct_UsingSpy()
    {
        // Arrange - Create a spy
        var mockService = new Mock<DiscountService>();
        mockService.CallBase = true;
        
        var electronicsProduct = new Product("Laptop", 1000.00m, "Electronics");
        var bookProduct = new Product("Book", 50.00m, "Books");

        // Act
        mockService.Object.ApplyDiscount(electronicsProduct, 1000.00m);
        mockService.Object.ApplyDiscount(bookProduct, 50.00m);

        // Assert - Verify it was called with electronics product
        mockService.Verify(
            s => s.ApplyDiscount(
                It.Is<Product>(p => p.Category == "Electronics"), 
                It.IsAny<decimal>()), 
            Times.Once);
        
        // Assert - Verify it was called with books product
        mockService.Verify(
            s => s.ApplyDiscount(
                It.Is<Product>(p => p.Category == "Books"), 
                It.IsAny<decimal>()), 
            Times.Once);
    }

    [Fact]
    public void ApplyDiscount_VerifyNeverCalledWithSpecificCategory_UsingSpy()
    {
        // Arrange - Create a spy
        var mockService = new Mock<DiscountService>();
        mockService.CallBase = true;
        
        var product = new Product("Laptop", 1000.00m, "Electronics");

        // Act
        mockService.Object.ApplyDiscount(product, 1000.00m);

        // Assert - Verify it was never called with Food category
        mockService.Verify(
            s => s.ApplyDiscount(
                It.Is<Product>(p => p.Category == "Food"), 
                It.IsAny<decimal>()), 
            Times.Never);
    }

    [Fact]
    public void ApplyDiscount_SpyVerifiesOrderOfCalls()
    {
        // Arrange - Create a spy
        var mockService = new Mock<DiscountService>();
        mockService.CallBase = true;
        
        var callSequence = new List<string>();
        mockService.Setup(s => s.ApplyDiscount(It.IsAny<Product>(), It.IsAny<decimal>()))
            .Callback<Product, decimal>((p, price) => callSequence.Add(p.Category))
            .CallBase();

        var product1 = new Product("Laptop", 1000.00m, "Electronics");
        var product2 = new Product("Book", 50.00m, "Books");
        var product3 = new Product("Shirt", 100.00m, "Clothing");

        // Act
        mockService.Object.ApplyDiscount(product1, 1000.00m);
        mockService.Object.ApplyDiscount(product2, 50.00m);
        mockService.Object.ApplyDiscount(product3, 100.00m);

        // Assert - Verify the order of calls
        Assert.Equal(3, callSequence.Count);
        Assert.Equal("Electronics", callSequence[0]);
        Assert.Equal("Books", callSequence[1]);
        Assert.Equal("Clothing", callSequence[2]);
    }
}
