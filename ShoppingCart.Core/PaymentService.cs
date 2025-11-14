namespace ShoppingCart.Core;

public class PaymentService : IPaymentService
{
    private string? _lastTransactionId;

    public bool ProcessPayment(decimal amount)
    {
        if (amount <= 0)
            return false;

        // Simulate payment processing
        _lastTransactionId = Guid.NewGuid().ToString();
        return true;
    }

    public string GetLastTransactionId()
    {
        return _lastTransactionId ?? string.Empty;
    }
}
