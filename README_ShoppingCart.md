# Sistem de Gestionare a Coșului de Cumpărături

Proiect C# pentru testare avansată cu XUnit, demonstrând utilizarea de mock-uri și spy-uri.

## Structura Proiectului

### ShoppingCart.Core
Proiectul principal conține următoarele clase:

- **Product** - Reprezentarea unui produs cu nume, preț și categorie
- **ShoppingCart** - Gestionează coșul de cumpărături (adaugă, șterge, calculează total)
- **DiscountService** - Aplică reduceri în funcție de categorie
- **PaymentService** - Procesează plățile

### ShoppingCart.Tests
Proiectul de teste conține teste complete pentru toate clasele, incluzând:

- **ProductTests** - Teste pentru validarea produselor
- **ShoppingCartTests** - Teste pentru coș cu mock-uri pentru DiscountService
- **DiscountServiceTests** - Teste pentru reduceri cu spy-uri pentru verificare
- **PaymentServiceTests** - Teste cu mock-uri pentru simularea plăților

## Funcționalități Testate

### 1. Product
- ✅ Validare nume, preț, categorie
- ✅ Teste de graniță pentru prețuri (0, 0.01, 999999.99)
- ✅ Validare pentru valori negative
- ✅ Teste pentru egalitate între produse

### 2. ShoppingCart
- ✅ Adăugare produse (cu cantități diferite)
- ✅ Ștergere produse
- ✅ Calculare total cu discount
- ✅ Teste de graniță pentru cantități (0, 1, 100, 1000)
- ✅ Gestionare produse duplicate
- ✅ Mock pentru DiscountService

### 3. DiscountService
- ✅ Aplicare reduceri pe categorii:
  - Electronics: 10%
  - Clothing: 15%
  - Books: 5%
  - Food: 0%
- ✅ Teste de graniță pentru prețuri
- ✅ **Spy** pentru verificarea apelărilor metodelor
- ✅ Verificare ordinea apelărilor
- ✅ Teste case-insensitive pentru categorii

### 4. PaymentService
- ✅ Procesare plăți valide
- ✅ Generare ID-uri unice de tranzacție
- ✅ Teste de graniță pentru sume (0, 0.01, 999999.99)
- ✅ **Mock complet** pentru simularea serviciului de plată
- ✅ Mock cu secvențe de returnări (retry logic)
- ✅ Mock cu callback pentru capturarea parametrilor
- ✅ Mock cu excepții

## Tehnici de Testare Utilizate

### Mock-uri (Moq)
Mock-uri complete pentru:
- `IDiscountService` în testele pentru ShoppingCart
- `IPaymentService` pentru simularea plăților
- Configurare comportament condiționat (ex: succes/eșec bazat pe sumă)

### Spy-uri
Spy-uri implementate cu Moq pentru:
- Verificarea că `ApplyDiscount` a fost apelat de ShoppingCart
- Verificarea numărului de apelări
- Verificarea parametrilor specifici
- Verificarea ordinii apelărilor

### Teste de Graniță
- Prețuri: 0, 0.01, 999999.99, -1
- Cantități: 0, 1, 10, 100, 1000
- Reduceri: 0%, 5%, 10%, 15%, 100%

## Rulare Proiect

### Compilare
```bash
dotnet build ShoppingCart.sln
```

### Rulare Teste
```bash
dotnet test ShoppingCart.sln
```

### Rulare Teste cu Raport Detaliat
```bash
dotnet test ShoppingCart.sln --verbosity detailed
```

### Rulare Teste Specifice
```bash
# Doar testele pentru Product
dotnet test --filter "FullyQualifiedName~ProductTests"

# Doar testele pentru ShoppingCart
dotnet test --filter "FullyQualifiedName~ShoppingCartTests"

# Doar testele pentru DiscountService
dotnet test --filter "FullyQualifiedName~DiscountServiceTests"

# Doar testele pentru PaymentService
dotnet test --filter "FullyQualifiedName~PaymentServiceTests"
```

## Statistici Teste

- **Total teste**: 20 (27 execuții cu theory tests)
- **ProductTests**: 4 teste
- **ShoppingCartTests**: 6 teste
- **DiscountServiceTests**: 4 teste (incluzând spy-uri)
- **PaymentServiceTests**: 6 teste (incluzând mock-uri)

## Exemple de Utilizare

### Utilizare Normală
```csharp
var discountService = new DiscountService();
var cart = new ShoppingCart(discountService);

var laptop = new Product("Laptop", 1500.00m, "Electronics");
var book = new Product("C# Programming", 50.00m, "Books");

cart.AddProduct(laptop, 1);
cart.AddProduct(book, 2);

var total = cart.CalculateTotal(); // 1500 * 0.9 + 100 * 0.95 = 1350 + 95 = 1445

var paymentService = new PaymentService();
var success = paymentService.ProcessPayment(total);
var transactionId = paymentService.GetLastTransactionId();
```

### Cu Mock-uri în Teste
```csharp
var mockDiscountService = new Mock<IDiscountService>();
mockDiscountService.Setup(s => s.ApplyDiscount(It.IsAny<Product>(), It.IsAny<decimal>()))
    .Returns<Product, decimal>((p, price) => price * 0.9m);

var cart = new ShoppingCart(mockDiscountService.Object);
// ... teste
```

### Cu Spy-uri în Teste
```csharp
var spyDiscountService = new Mock<DiscountService>();
spyDiscountService.CallBase = true; // Spy - apelează implementarea reală

// Folosește spy-ul
spyDiscountService.Object.ApplyDiscount(product, 100);

// Verifică că a fost apelat
spyDiscountService.Verify(s => s.ApplyDiscount(It.IsAny<Product>(), It.IsAny<decimal>()), Times.Once);
```

## Tehnologii
- .NET 9.0
- C# 13
- XUnit 2.8.2
- Moq 4.20.72

## Autor
Proiect realizat pentru curs TAS (Testarea și Asigurarea Calității Software)
