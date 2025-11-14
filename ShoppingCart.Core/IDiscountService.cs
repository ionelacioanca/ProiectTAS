namespace ShoppingCart.Core;

public interface IDiscountService
{
    decimal ApplyDiscount(Product product, decimal price);
}
