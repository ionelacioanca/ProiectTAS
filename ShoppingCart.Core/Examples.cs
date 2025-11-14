using ShoppingCart.Core;

namespace ShoppingCart.Examples;

/// <summary>
/// Exemple de utilizare a sistemului de coș de cumpărături
/// </summary>
public class ShoppingCartExamples
{
    public static void Main()
    {
        Console.WriteLine("=== Sistem de Gestionare Coș de Cumpărături ===\n");

        // Exemplu 1: Utilizare de bază
        BasicUsageExample();
        
        Console.WriteLine("\n" + new string('-', 50) + "\n");
        
        // Exemplu 2: Aplicarea reducerilor
        DiscountExample();
        
        Console.WriteLine("\n" + new string('-', 50) + "\n");
        
        // Exemplu 3: Procesare plată
        PaymentExample();
        
        Console.WriteLine("\n" + new string('-', 50) + "\n");
        
        // Exemplu 4: Coș complex
        ComplexCartExample();
    }

    static void BasicUsageExample()
    {
        Console.WriteLine("EXEMPLU 1: Utilizare de Bază");
        Console.WriteLine();

        // Creăm serviciul de reduceri și coșul
        var discountService = new DiscountService();
        var cart = new Core.ShoppingCart(discountService);

        // Creăm produse
        var laptop = new Product("Laptop Dell XPS", 1500.00m, "Electronics");
        var mouse = new Product("Mouse Logitech", 50.00m, "Electronics");
        var book = new Product("Clean Code", 45.00m, "Books");

        // Adăugăm produse în coș
        cart.AddProduct(laptop, 1);
        cart.AddProduct(mouse, 2);
        cart.AddProduct(book, 1);

        // Afișăm conținutul coșului
        Console.WriteLine("Produse în coș:");
        foreach (var item in cart.Items)
        {
            Console.WriteLine($"  - {item.Product.Name} x{item.Quantity} @ {item.Product.Price:C} = {item.Product.Price * item.Quantity:C}");
        }

        Console.WriteLine($"\nTotal articole: {cart.GetTotalItems()}");
        Console.WriteLine($"Total de plată (cu reduceri): {cart.CalculateTotal():C}");
    }

    static void DiscountExample()
    {
        Console.WriteLine("EXEMPLU 2: Aplicarea Reducerilor");
        Console.WriteLine();

        var discountService = new DiscountService();
        var cart = new Core.ShoppingCart(discountService);

        // Produse din categorii diferite
        var laptop = new Product("Laptop", 1000.00m, "Electronics");      // 10% discount
        var shirt = new Product("Tricou", 100.00m, "Clothing");           // 15% discount
        var book = new Product("C# Programming", 50.00m, "Books");        // 5% discount
        var apple = new Product("Măr", 5.00m, "Food");                    // 0% discount

        cart.AddProduct(laptop, 1);
        cart.AddProduct(shirt, 2);
        cart.AddProduct(book, 1);
        cart.AddProduct(apple, 10);

        Console.WriteLine("Reduceri pe categorii:");
        Console.WriteLine("  - Electronics: 10%");
        Console.WriteLine("  - Clothing: 15%");
        Console.WriteLine("  - Books: 5%");
        Console.WriteLine("  - Food: 0%");
        Console.WriteLine();

        Console.WriteLine("Detalii coș:");
        foreach (var item in cart.Items)
        {
            var originalPrice = item.Product.Price * item.Quantity;
            var discountedPrice = discountService.ApplyDiscount(item.Product, originalPrice);
            var savedAmount = originalPrice - discountedPrice;
            
            Console.WriteLine($"  - {item.Product.Name} ({item.Product.Category})");
            Console.WriteLine($"    Preț original: {originalPrice:C}");
            Console.WriteLine($"    Preț cu reducere: {discountedPrice:C}");
            Console.WriteLine($"    Economisit: {savedAmount:C}");
            Console.WriteLine();
        }

        var total = cart.CalculateTotal();
        Console.WriteLine($"Total final: {total:C}");
    }

    static void PaymentExample()
    {
        Console.WriteLine("EXEMPLU 3: Procesare Plată");
        Console.WriteLine();

        var discountService = new DiscountService();
        var cart = new Core.ShoppingCart(discountService);
        var paymentService = new PaymentService();

        // Adăugăm produse
        cart.AddProduct(new Product("Laptop", 1200.00m, "Electronics"), 1);
        cart.AddProduct(new Product("Mouse", 30.00m, "Electronics"), 1);

        var total = cart.CalculateTotal();
        Console.WriteLine($"Total de plată: {total:C}");
        Console.WriteLine();

        // Procesăm plata
        Console.WriteLine("Se procesează plata...");
        var success = paymentService.ProcessPayment(total);

        if (success)
        {
            var transactionId = paymentService.GetLastTransactionId();
            Console.WriteLine("✓ Plată realizată cu succes!");
            Console.WriteLine($"  ID Tranzacție: {transactionId}");
            
            // Golim coșul după plată
            cart.Clear();
            Console.WriteLine("\n✓ Coș golit. Mulțumim pentru cumpărături!");
        }
        else
        {
            Console.WriteLine("✗ Plata a eșuat. Vă rugăm să încercați din nou.");
        }
    }

    static void ComplexCartExample()
    {
        Console.WriteLine("EXEMPLU 4: Coș Complex cu Modificări");
        Console.WriteLine();

        var discountService = new DiscountService();
        var cart = new Core.ShoppingCart(discountService);

        // Creăm produse
        var laptop = new Product("Laptop", 1000.00m, "Electronics");
        var book1 = new Product("Design Patterns", 50.00m, "Books");
        var book2 = new Product("Clean Architecture", 45.00m, "Books");
        var shirt = new Product("Tricou", 80.00m, "Clothing");

        // Adăugăm produse
        Console.WriteLine("Adăugăm produse în coș...");
        cart.AddProduct(laptop, 1);
        cart.AddProduct(book1, 2);
        cart.AddProduct(book2, 1);
        cart.AddProduct(shirt, 3);

        Console.WriteLine($"Articole în coș: {cart.GetTotalItems()}");
        Console.WriteLine($"Total: {cart.CalculateTotal():C}\n");

        // Adăugăm mai multe cărți
        Console.WriteLine("Adăugăm încă o carte 'Design Patterns'...");
        cart.AddProduct(book1, 1);
        Console.WriteLine($"Articole în coș: {cart.GetTotalItems()}");
        Console.WriteLine($"Total: {cart.CalculateTotal():C}\n");

        // Eliminăm un tricou
        Console.WriteLine("Eliminăm un tricou...");
        cart.RemoveProduct(shirt, 1);
        Console.WriteLine($"Articole în coș: {cart.GetTotalItems()}");
        Console.WriteLine($"Total: {cart.CalculateTotal():C}\n");

        // Afișăm starea finală
        Console.WriteLine("Stare finală coș:");
        foreach (var item in cart.Items)
        {
            Console.WriteLine($"  - {item.Product.Name} x{item.Quantity} @ {item.Product.Price:C}");
        }
        Console.WriteLine($"\nTotal final (cu reduceri): {cart.CalculateTotal():C}");

        // Configurăm o reducere personalizată
        Console.WriteLine("\n--- Aplicăm o reducere specială de 20% pentru Electronics ---");
        discountService.SetCategoryDiscount("Electronics", 0.20m);
        Console.WriteLine($"Total nou: {cart.CalculateTotal():C}");
    }
}
