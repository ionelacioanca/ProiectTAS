namespace ShoppingCart.Core;

public class DiscountService : IDiscountService
{
    private readonly Dictionary<string, decimal> _categoryDiscounts;

    public DiscountService()
    {
        _categoryDiscounts = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
        {
            { "Electronics", 0.10m },  // 10% discount
            { "Clothing", 0.15m },     // 15% discount
            { "Books", 0.05m },        // 5% discount
            { "Food", 0.00m }          // No discount
        };
    }

    public virtual decimal ApplyDiscount(Product product, decimal price)
    {
        if (_categoryDiscounts.TryGetValue(product.Category, out var discountRate))
        {
            return price * (1 - discountRate);
        }
        
        return price; // No discount for unknown categories
    }

    public void SetCategoryDiscount(string category, decimal discountRate)
    {
        if (discountRate < 0 || discountRate > 1)
            throw new ArgumentException("Discount rate must be between 0 and 1", nameof(discountRate));
        
        _categoryDiscounts[category] = discountRate;
    }
}
