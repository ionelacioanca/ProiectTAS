namespace ShoppingCart.Core;

public class Product
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }

    public Product(string name, decimal price, string category)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));
        
        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));
        
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category cannot be empty", nameof(category));

        Name = name;
        Price = price;
        Category = category;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Product other)
        {
            return Name == other.Name && 
                   Price == other.Price && 
                   Category == other.Category;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Price, Category);
    }
}
