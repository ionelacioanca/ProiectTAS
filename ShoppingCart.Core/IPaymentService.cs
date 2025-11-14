namespace ShoppingCart.Core;

public interface IPaymentService
{
    bool ProcessPayment(decimal amount);
    string GetLastTransactionId();
}
