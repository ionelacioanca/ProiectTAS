using ShoppingCart.Core;
using Moq;
using Xunit;

namespace ShoppingCart.Tests;

public class DiscountServiceTests
{
    [Theory]
    [InlineData("Electronics", 1000, 900)]    // 10% discount
    [InlineData("Clothing", 100, 85)]         // 15% discount
    [InlineData("Books", 50, 47.50)]          // 5% discount
    [InlineData("Food", 5, 5)]                // 0% discount
    public void ApplyDiscount_DifferentCategories_ShouldApplyCorrectDiscount(string category, decimal price, decimal expected)
    {
        // Arrange
        var service = new DiscountService();
        var product = new Product("Test", price, category);

        // Act
        var discountedPrice = service.ApplyDiscount(product, price);

        // Assert
        Assert.Equal(expected, discountedPrice);
    }

    [Fact]
    public void ApplyDiscount_CalledMultipleTimes_UsingSpy()
    {
        // Arrange - Spy cu Moq
        var spyService = new Mock<DiscountService>();
        spyService.CallBase = true; // Spy - apelează implementarea reală
        
        var product1 = new Product("Laptop", 1000.00m, "Electronics");
        var product2 = new Product("Book", 50.00m, "Books");
        var product3 = new Product("Shirt", 100.00m, "Clothing");

        // Act
        spyService.Object.ApplyDiscount(product1, 1000.00m);
        spyService.Object.ApplyDiscount(product2, 50.00m);
        spyService.Object.ApplyDiscount(product3, 100.00m);

        // Assert - Verifică că a fost apelat de 3 ori
        spyService.Verify(s => s.ApplyDiscount(It.IsAny<Product>(), It.IsAny<decimal>()), Times.Exactly(3));
    }

    [Fact]
    public void ApplyDiscount_VerifyCalledWithSpecificCategory_UsingSpy()
    {
        // Arrange - Spy
        var spyService = new Mock<DiscountService>();
        spyService.CallBase = true;
        
        var electronicsProduct = new Product("Laptop", 1000.00m, "Electronics");
        var bookProduct = new Product("Book", 50.00m, "Books");

        // Act
        spyService.Object.ApplyDiscount(electronicsProduct, 1000.00m);
        spyService.Object.ApplyDiscount(bookProduct, 50.00m);

        // Assert - Verifică apelare cu Electronics
        spyService.Verify(s => s.ApplyDiscount(It.Is<Product>(p => p.Category == "Electronics"), It.IsAny<decimal>()), Times.Once);
        
        // Assert - Verifică că nu a fost apelat cu Food
        spyService.Verify(s => s.ApplyDiscount(It.Is<Product>(p => p.Category == "Food"), It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public void ApplyDiscount_SpyVerifiesOrderOfCalls()
    {
        // Arrange - Spy cu callback pentru a captura ordinea
        var spyService = new Mock<DiscountService>();
        spyService.CallBase = true;
        
        var callSequence = new List<string>();
        spyService.Setup(s => s.ApplyDiscount(It.IsAny<Product>(), It.IsAny<decimal>()))
            .Callback<Product, decimal>((p, price) => callSequence.Add(p.Category))
            .CallBase();

        var product1 = new Product("Laptop", 1000.00m, "Electronics");
        var product2 = new Product("Book", 50.00m, "Books");

        // Act
        spyService.Object.ApplyDiscount(product1, 1000.00m);
        spyService.Object.ApplyDiscount(product2, 50.00m);

        // Assert - Verifică ordinea
        Assert.Equal("Electronics", callSequence[0]);
        Assert.Equal("Books", callSequence[1]);
    }
}
