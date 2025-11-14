using ShoppingCart.Core;
using Moq;
using Xunit;

namespace ShoppingCart.Tests;

public class PaymentServiceTests
{
    [Fact]
    public void ProcessPayment_WithValidAmount_ShouldReturnTrue()
    {
        // Arrange
        var service = new PaymentService();

        // Act
        var result = service.ProcessPayment(100.00m);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ProcessPayment_WithValidAmount_ShouldGenerateTransactionId()
    {
        // Arrange
        var service = new PaymentService();

        // Act
        service.ProcessPayment(100.00m);
        var transactionId = service.GetLastTransactionId();

        // Assert
        Assert.NotNull(transactionId);
        Assert.NotEmpty(transactionId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100.50)]
    public void ProcessPayment_WithInvalidAmount_ShouldReturnFalse(decimal amount)
    {
        // Arrange
        var service = new PaymentService();

        // Act
        var result = service.ProcessPayment(amount);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(0.01)]    // Minimum valid
    [InlineData(1)]       // Small amount
    [InlineData(1000)]    // Medium amount
    [InlineData(999999.99)] // Large amount
    public void ProcessPayment_BoundaryAmounts_ShouldReturnTrue(decimal amount)
    {
        // Arrange
        var service = new PaymentService();

        // Act
        var result = service.ProcessPayment(amount);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ProcessPayment_MultipleTimes_ShouldGenerateUniqueTransactionIds()
    {
        // Arrange
        var service = new PaymentService();
        var transactionIds = new List<string>();

        // Act
        for (int i = 0; i < 5; i++)
        {
            service.ProcessPayment(100.00m);
            transactionIds.Add(service.GetLastTransactionId());
        }

        // Assert
        var uniqueIds = transactionIds.Distinct().ToList();
        Assert.Equal(transactionIds.Count, uniqueIds.Count);
    }

    [Fact]
    public void GetLastTransactionId_BeforeAnyPayment_ShouldReturnEmpty()
    {
        // Arrange
        var service = new PaymentService();

        // Act
        var transactionId = service.GetLastTransactionId();

        // Assert
        Assert.Empty(transactionId);
    }

    [Fact]
    public void GetLastTransactionId_AfterFailedPayment_ShouldReturnPreviousTransactionId()
    {
        // Arrange
        var service = new PaymentService();
        service.ProcessPayment(100.00m);
        var firstTransactionId = service.GetLastTransactionId();

        // Act
        service.ProcessPayment(-10.00m); // Failed payment
        var lastTransactionId = service.GetLastTransactionId();

        // Assert
        Assert.Equal(firstTransactionId, lastTransactionId);
    }

    [Fact]
    public void Mock_ProcessPayment_ShouldReturnConfiguredValue()
    {
        // Arrange
        var mockPaymentService = new Mock<IPaymentService>();
        mockPaymentService.Setup(s => s.ProcessPayment(It.IsAny<decimal>())).Returns(true);

        // Act
        var result = mockPaymentService.Object.ProcessPayment(100.00m);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Mock_ProcessPayment_CanSimulateFailure()
    {
        // Arrange
        var mockPaymentService = new Mock<IPaymentService>();
        mockPaymentService.Setup(s => s.ProcessPayment(It.IsAny<decimal>())).Returns(false);

        // Act
        var result = mockPaymentService.Object.ProcessPayment(100.00m);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Mock_ProcessPayment_CanReturnDifferentValuesBasedOnAmount()
    {
        // Arrange
        var mockPaymentService = new Mock<IPaymentService>();
        mockPaymentService.Setup(s => s.ProcessPayment(It.Is<decimal>(amt => amt > 0 && amt <= 1000)))
            .Returns(true);
        mockPaymentService.Setup(s => s.ProcessPayment(It.Is<decimal>(amt => amt > 1000)))
            .Returns(false);

        // Act
        var resultSmall = mockPaymentService.Object.ProcessPayment(500.00m);
        var resultLarge = mockPaymentService.Object.ProcessPayment(1500.00m);

        // Assert
        Assert.True(resultSmall);
        Assert.False(resultLarge);
    }

    [Fact]
    public void Mock_ProcessPayment_VerifyWasCalled()
    {
        // Arrange
        var mockPaymentService = new Mock<IPaymentService>();
        mockPaymentService.Setup(s => s.ProcessPayment(It.IsAny<decimal>())).Returns(true);

        // Act
        mockPaymentService.Object.ProcessPayment(100.00m);

        // Assert
        mockPaymentService.Verify(s => s.ProcessPayment(100.00m), Times.Once);
    }

    [Fact]
    public void Mock_ProcessPayment_VerifyNeverCalled()
    {
        // Arrange
        var mockPaymentService = new Mock<IPaymentService>();

        // Assert
        mockPaymentService.Verify(s => s.ProcessPayment(It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public void Mock_ProcessPayment_VerifyCalledWithSpecificAmount()
    {
        // Arrange
        var mockPaymentService = new Mock<IPaymentService>();
        mockPaymentService.Setup(s => s.ProcessPayment(It.IsAny<decimal>())).Returns(true);

        // Act
        mockPaymentService.Object.ProcessPayment(150.75m);

        // Assert
        mockPaymentService.Verify(s => s.ProcessPayment(150.75m), Times.Once);
        mockPaymentService.Verify(s => s.ProcessPayment(It.Is<decimal>(amt => amt > 100)), Times.Once);
    }

    [Fact]
    public void Mock_GetLastTransactionId_CanReturnConfiguredValue()
    {
        // Arrange
        var mockPaymentService = new Mock<IPaymentService>();
        mockPaymentService.Setup(s => s.GetLastTransactionId()).Returns("MOCK-TRANSACTION-12345");

        // Act
        var transactionId = mockPaymentService.Object.GetLastTransactionId();

        // Assert
        Assert.Equal("MOCK-TRANSACTION-12345", transactionId);
    }

    [Fact]
    public void Mock_IntegrationWithShoppingCart_ShouldProcessPaymentCorrectly()
    {
        // Arrange
        var mockPaymentService = new Mock<IPaymentService>();
        var mockDiscountService = new Mock<IDiscountService>();
        
        mockPaymentService.Setup(s => s.ProcessPayment(It.IsAny<decimal>())).Returns(true);
        mockPaymentService.Setup(s => s.GetLastTransactionId()).Returns("TXN-001");
        mockDiscountService.Setup(s => s.ApplyDiscount(It.IsAny<Product>(), It.IsAny<decimal>()))
            .Returns<Product, decimal>((p, price) => price);

        var cart = new Core.ShoppingCart(mockDiscountService.Object);
        cart.AddProduct(new Product("Laptop", 1000.00m, "Electronics"));

        // Act
        var total = cart.CalculateTotal();
        var paymentResult = mockPaymentService.Object.ProcessPayment(total);
        var transactionId = mockPaymentService.Object.GetLastTransactionId();

        // Assert
        Assert.True(paymentResult);
        Assert.Equal("TXN-001", transactionId);
        mockPaymentService.Verify(s => s.ProcessPayment(1000.00m), Times.Once);
    }

    [Fact]
    public void Mock_ProcessPayment_CanThrowException()
    {
        // Arrange
        var mockPaymentService = new Mock<IPaymentService>();
        mockPaymentService.Setup(s => s.ProcessPayment(It.IsAny<decimal>()))
            .Throws(new InvalidOperationException("Payment gateway unavailable"));

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            mockPaymentService.Object.ProcessPayment(100.00m));
        Assert.Contains("Payment gateway unavailable", exception.Message);
    }

    [Fact]
    public void Mock_ProcessPayment_SequenceOfReturns()
    {
        // Arrange
        var mockPaymentService = new Mock<IPaymentService>();
        mockPaymentService.SetupSequence(s => s.ProcessPayment(It.IsAny<decimal>()))
            .Returns(false)  // First call fails
            .Returns(false)  // Second call fails
            .Returns(true);  // Third call succeeds

        // Act
        var result1 = mockPaymentService.Object.ProcessPayment(100.00m);
        var result2 = mockPaymentService.Object.ProcessPayment(100.00m);
        var result3 = mockPaymentService.Object.ProcessPayment(100.00m);

        // Assert
        Assert.False(result1);
        Assert.False(result2);
        Assert.True(result3);
    }

    [Fact]
    public void Mock_ProcessPayment_WithCallback()
    {
        // Arrange
        var mockPaymentService = new Mock<IPaymentService>();
        var capturedAmount = 0m;
        
        mockPaymentService.Setup(s => s.ProcessPayment(It.IsAny<decimal>()))
            .Callback<decimal>(amount => capturedAmount = amount)
            .Returns(true);

        // Act
        mockPaymentService.Object.ProcessPayment(250.50m);

        // Assert
        Assert.Equal(250.50m, capturedAmount);
    }
}
