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
        Assert.NotEmpty(service.GetLastTransactionId());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ProcessPayment_WithInvalidAmount_ShouldReturnFalse(decimal amount)
    {
        // Arrange
        var service = new PaymentService();

        // Act
        var result = service.ProcessPayment(amount);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Mock_ProcessPayment_ShouldReturnConfiguredValue()
    {
        // Arrange - Mock pentru simulare
        var mockPaymentService = new Mock<IPaymentService>();
        mockPaymentService.Setup(s => s.ProcessPayment(It.IsAny<decimal>())).Returns(true);
        mockPaymentService.Setup(s => s.GetLastTransactionId()).Returns("MOCK-TXN-123");

        // Act
        var result = mockPaymentService.Object.ProcessPayment(100.00m);
        var txnId = mockPaymentService.Object.GetLastTransactionId();

        // Assert
        Assert.True(result);
        Assert.Equal("MOCK-TXN-123", txnId);
    }

    [Fact]
    public void Mock_ProcessPayment_CanReturnDifferentValuesBasedOnAmount()
    {
        // Arrange - Mock cu comportament condiționat
        var mockPaymentService = new Mock<IPaymentService>();
        mockPaymentService.Setup(s => s.ProcessPayment(It.Is<decimal>(amt => amt > 0 && amt <= 1000))).Returns(true);
        mockPaymentService.Setup(s => s.ProcessPayment(It.Is<decimal>(amt => amt > 1000))).Returns(false);

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
        // Arrange - Mock cu verificare
        var mockPaymentService = new Mock<IPaymentService>();
        mockPaymentService.Setup(s => s.ProcessPayment(It.IsAny<decimal>())).Returns(true);

        // Act
        mockPaymentService.Object.ProcessPayment(250.00m);
        mockPaymentService.Object.ProcessPayment(150.00m);

        // Assert - Verifică că a fost apelat de 2 ori
        mockPaymentService.Verify(s => s.ProcessPayment(It.IsAny<decimal>()), Times.Exactly(2));
        mockPaymentService.Verify(s => s.ProcessPayment(250.00m), Times.Once);
    }

    [Fact]
    public void Mock_ProcessPayment_WithCallback()
    {
        // Arrange - Mock cu callback pentru capturarea parametrilor
        var mockPaymentService = new Mock<IPaymentService>();
        var capturedAmount = 0m;
        
        mockPaymentService.Setup(s => s.ProcessPayment(It.IsAny<decimal>()))
            .Callback<decimal>(amount => capturedAmount = amount)
            .Returns(true);

        // Act
        mockPaymentService.Object.ProcessPayment(250.50m);

        // Assert - Verifică că suma a fost capturată corect
        Assert.Equal(250.50m, capturedAmount);
    }
}
