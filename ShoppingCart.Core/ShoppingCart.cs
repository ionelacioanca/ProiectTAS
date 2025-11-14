namespace ShoppingCart.Core;

public class ShoppingCart
{
    private readonly List<CartItem> _items;
    private readonly IDiscountService _discountService;

    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

    public ShoppingCart(IDiscountService discountService)
    {
        _items = new List<CartItem>();
        _discountService = discountService ?? throw new ArgumentNullException(nameof(discountService));
    }

    public void AddProduct(Product product, int quantity = 1)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));
        
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        var existingItem = _items.FirstOrDefault(i => i.Product.Equals(product));
        
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            _items.Add(new CartItem(product, quantity));
        }
    }

    public void RemoveProduct(Product product, int quantity = 1)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));
        
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        var existingItem = _items.FirstOrDefault(i => i.Product.Equals(product));
        
        if (existingItem == null)
            throw new InvalidOperationException("Product not found in cart");

        existingItem.Quantity -= quantity;
        
        if (existingItem.Quantity <= 0)
        {
            _items.Remove(existingItem);
        }
    }

    public void Clear()
    {
        _items.Clear();
    }

    public decimal CalculateTotal()
    {
        decimal total = 0;

        foreach (var item in _items)
        {
            var itemPrice = item.Product.Price * item.Quantity;
            var discountedPrice = _discountService.ApplyDiscount(item.Product, itemPrice);
            total += discountedPrice;
        }

        return total;
    }

    public int GetTotalItems()
    {
        return _items.Sum(i => i.Quantity);
    }
}

public class CartItem
{
    public Product Product { get; }
    public int Quantity { get; set; }

    public CartItem(Product product, int quantity)
    {
        Product = product ?? throw new ArgumentNullException(nameof(product));
        
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));
        
        Quantity = quantity;
    }
}
