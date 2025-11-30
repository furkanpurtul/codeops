# CodeOps.Domain.Abstractions

A comprehensive, production-ready library of domain-driven design (DDD) building blocks for .NET 8+ applications. This library provides strongly-typed base classes and utilities for implementing tactical DDD patterns with built-in invariant validation, domain events, and more.

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

---

## Table of Contents

- [Features](#-features)
- [Installation](#-installation)
- [Quick Start](#-quick-start)
- [Core Concepts](#-core-concepts)
  - [Entities](#entities)
  - [Aggregate Roots](#aggregate-roots)
  - [Value Objects](#value-objects)
  - [Strongly Typed IDs](#strongly-typed-ids)
  - [Enumerations](#enumerations)
  - [Rules & Validation](#rules--validation)
  - [Domain Events](#domain-events)
- [Advanced Topics](#-advanced-topics)
- [Design Principles](#-design-principles)
- [API Reference](#-api-reference)

---

## Features

- **Tactical DDD Patterns**: Entity, Aggregate Root, Value Object, Domain Events
- **Strong Typing**: Strongly-typed IDs with compile-time safety
- **Built-in Validation**: Rule-based invariant enforcement with `RuleEngine`
- **Smart Enumerations**: Type-safe alternatives to C# enums with rich behavior
- **Domain Events**: First-class support for domain event sourcing
- **Performance Optimized**: Cached equality components, lock-free implementations
- **Thread-Safe**: Concurrent-safe caching and immutable designs
- **Zero Dependencies**: No external package dependencies
- **Clean Architecture**: Follows SOLID principles and DDD best practices

---

## Quick Start

### Creating a Simple Aggregate

```csharp
using CodeOps.Domain.Abstractions;

// 1. Define a strongly-typed ID
public sealed record OrderId : StronglyTypedId<Guid>
{
    public OrderId(Guid value) : base(value) { }
    public static OrderId New() => new(Guid.NewGuid());
}

// 2. Create your aggregate root
public sealed class Order : AggregateRoot<Order, OrderId>
{
    private readonly List<OrderLine> _lines = [];
    public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();
    public OrderStatus Status { get; private set; }

    private Order(OrderId id, IEnumerable<OrderLine> lines) 
        : base(id, [new AtLeastOneLineRule(), new AllLinesSameCurrencyRule()])
    {
        Status = OrderStatus.Pending;
        _lines.AddRange(lines);
        // Constructor validates invariants automatically
    }

    public static Order Create(OrderId id, IEnumerable<OrderLine> lines)
    {
        return new Order(id, lines);
    }

    public void Cancel()
    {
        RuleEngine.Validate(this, new OrderCannotBeCancelledRule());
        Status = OrderStatus.Cancelled;
        RaiseDomainEvent(new OrderCancelledEvent(Id));
    }
}

// 3. Define validation rules
public sealed class AtLeastOneLineRule : IRule<Order>
{
    public string Describe(Order context) => "Order must have at least one line.";
    public bool IsViolatedBy(Order context) => context.Lines.Count == 0;
}
```

### Creating a Value Object

```csharp
public sealed class Money : ValueObject<Money>
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency) 
        : base([new AmountNonNegativeRule(), new CurrencyIso4217Rule()])
    {
        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public static Money Of(decimal amount, string currency) 
        => new(amount, currency);

    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");
        return Of(a.Amount + b.Amount, a.Currency);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:N2} {Currency}";
}
```

---

## Core Concepts

### Entities

Entities are objects with a unique identity that persists over time, even as their attributes change.

#### Base Class: `Entity<TDerived, TId>`

```csharp
public abstract class Entity<TDerived, TId> : Validatable<TDerived>, IEntity<TId>
    where TDerived : Entity<TDerived, TId>
    where TId : class, IStronglyTypedId
```

#### Key Features

- **Identity-Based Equality**: Two entities are equal if they have the same ID and type
- **Transient Detection**: `IsTransient` property indicates if entity has been persisted
- **Proxy Support**: `GetUnproxiedType()` for ORM lazy-loading scenarios
- **Automatic Validation**: Enforces invariants through rule engine

#### Example

```csharp
public sealed class Customer : Entity<Customer, CustomerId>
{
    public string Name { get; private set; }
    public Email Email { get; private set; }

    private Customer(CustomerId id, string name, Email email) 
        : base(id, [new NameNotEmptyRule(), new EmailValidRule()])
    {
        Name = name;
        Email = email;
    }

    public static Customer Create(CustomerId id, string name, Email email)
    {
        return new Customer(id, name, email);
    }

    public void UpdateEmail(Email newEmail)
    {
        RuleEngine.Validate(this, new EmailCanBeChangedRule());
        Email = newEmail;
    }
}
```

#### Equality Behavior

```csharp
var customer1 = Customer.Create(customerId, "John", email);
var customer2 = Customer.Create(customerId, "John", email);

customer1 == customer2; // true - same ID
customer1.Equals(customer2); // true

var transient = Customer.Create(new CustomerId(Guid.Empty), "Jane", email);
transient.IsTransient; // true if ID is default value
```

---

### Aggregate Roots

Aggregates are clusters of domain objects that can be treated as a single unit. The aggregate root is the entry point.

#### Base Class: `AggregateRoot<TDerived, TId>`

```csharp
public abstract class AggregateRoot<TDerived, TId> : Entity<TDerived, TId>, IAggregateRoot<TId>
    where TDerived : AggregateRoot<TDerived, TId>
    where TId : class, IStronglyTypedId
```

#### Key Features

- **Domain Events**: Built-in domain event collection and dispatching
- **Optimistic Concurrency**: Version tracking for optimistic locking
- **Consistency Boundary**: Enforces transactional consistency within the aggregate
- **Event Sourcing Support**: Compatible with event sourcing architectures

#### Domain Events

```csharp
public sealed class Order : AggregateRoot<Order, OrderId>
{
    public void Ship(ShippingAddress address)
    {
        RuleEngine.Validate(this, new OrderCanBeShippedRule());
        
        Status = OrderStatus.Shipped;
        ShippedAt = DateTime.UtcNow;
        
        // Raise domain event
        RaiseDomainEvent(new OrderShippedEvent(Id, address, ShippedAt));
        // Version is automatically incremented
    }
}

```

#### Version Management

```csharp
var order = Order.Create(orderId, lines);
Console.WriteLine(order.Version); // 0

order.Ship(address); // Raises event
Console.WriteLine(order.Version); // 1

order.Complete(); // Raises another event
Console.WriteLine(order.Version); // 2

// Use version for optimistic concurrency
await repository.UpdateAsync(order, expectedVersion: order.Version);
```

---

### Value Objects

Value objects are immutable objects defined by their attributes rather than identity.

#### Base Class: `ValueObject<TDerived>`

```csharp
public abstract class ValueObject<TDerived> : Validatable<TDerived>
    where TDerived : ValueObject<TDerived>
```

#### Key Features

- **Structural Equality**: Compared by value, not reference
- **Immutability**: All value objects should be immutable
- **Cached Components**: Equality components cached for performance
- **Custom Comparers**: Support for custom equality comparison logic
- **Thread-Safe**: Lock-free concurrent initialization

#### Example: Address Value Object

```csharp
public sealed class Address : ValueObject<Address>
{
    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string ZipCode { get; }
    public string Country { get; }

    private Address(string street, string city, string state, 
                   string zipCode, string country)
        : base([new ValidAddressRule()])
    {
        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
        Country = country;
    }

    public static Address Create(string street, string city, 
                                 string state, string zipCode, string country)
    {
        return new Address(street, city, state, zipCode, country);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return ZipCode;
        yield return Country;
    }

    public override string ToString() 
        => $"{Street}, {City}, {State} {ZipCode}, {Country}";
}
```

#### Custom Equality Comparers

```csharp
public sealed class CaseInsensitiveText : ValueObject<CaseInsensitiveText>
{
    public string Value { get; }

    private CaseInsensitiveText(string value) : base([])
    {
        Value = value;
    }

    // Override to use case-insensitive comparison
    protected override IEqualityComparer<object?> EqualityComponentComparer 
        => StringComparer.OrdinalIgnoreCase;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
```

---

### Strongly Typed IDs

Strongly-typed identifiers prevent primitive obsession and provide type safety.

#### Base Record: `StronglyTypedId<TValue>`

```csharp
public abstract record StronglyTypedId<TValue> : IStronglyTypedId
    where TValue : notnull
```

#### Key Features

- **Type Safety**: Prevents mixing different ID types
- **Guid, Int, String Support**: Works with any non-nullable value type
- **JSON Serialization**: Built-in JSON converters included
- **Implicit Conversion**: Implicit conversion to underlying value
- **Default Detection**: `IsDefault()` method for transient entities

#### Examples

```csharp
// Guid-based ID
public sealed record OrderId : StronglyTypedId<Guid>
{
    public OrderId(Guid value) : base(value) { }
    public static OrderId New() => new(Guid.NewGuid());
    public static OrderId From(string value) => new(Guid.Parse(value));
}

// Integer-based ID
public sealed record CustomerId : StronglyTypedId<int>
{
    public CustomerId(int value) : base(value) { }
    public static CustomerId From(int value) => new(value);
}

// String-based ID
public sealed record TenantId : StronglyTypedId<string>
{
    public TenantId(string value) : base(value) { }
    public static TenantId From(string value) => new(value);
}

// Usage
var orderId = OrderId.New();
var customerId = CustomerId.From(12345);

// Type safety - compile error!
// Order order = repository.Find(customerId); // Won't compile!

// Correct usage
Order order = repository.Find(orderId);

// Implicit conversion
Guid guidValue = orderId; // Automatically converts
```

#### JSON Serialization

```csharp
// Built-in converters in Converters folder
public class StronglyTypedIdJsonConverterFactory : JsonConverterFactory
{
    // Automatically handles serialization/deserialization
}

// Configure in your Startup/Program.cs
services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new StronglyTypedIdJsonConverterFactory());
    });
```

---

### Enumerations

Smart enumerations provide type-safe, extensible alternatives to C# enums.

#### Base Class: `Enumeration<TEnumeration>`

```csharp
public abstract class Enumeration<TEnumeration> : 
    IComparable, IComparable<TEnumeration>, IEquatable<TEnumeration>
    where TEnumeration : Enumeration<TEnumeration>
```

#### Key Features

- **Rich Behavior**: Add methods and properties to enum values
- **Reflection-Based Discovery**: Automatically finds all declarations
- **Parse & TryParse**: FromValue/FromName methods with case-insensitive options
- **Type-Safe**: Prevents mixing different enumeration types
- **Comparable**: Supports ordering and comparison operations

#### Example: Order Status

```csharp
public sealed class OrderStatus : Enumeration<OrderStatus>
{
    // Declare enum values as static fields
    public static readonly OrderStatus Pending = new(1, "Pending");
    public static readonly OrderStatus Confirmed = new(2, "Confirmed");
    public static readonly OrderStatus Shipped = new(3, "Shipped");
    public static readonly OrderStatus Delivered = new(4, "Delivered");
    public static readonly OrderStatus Cancelled = new(5, "Cancelled");

    private OrderStatus(int value, string name) : base(value, name) { }

    // Add custom behavior
    public bool CanBeCancelled() => this == Pending || this == Confirmed;
    public bool IsFinalState() => this == Delivered || this == Cancelled;
    public bool CanShip() => this == Confirmed;
}

// Usage
var status = OrderStatus.Pending;

if (status.CanBeCancelled())
{
    order.Cancel();
}

// Parse from value
var fromValue = OrderStatus.FromValue(1); // Returns Pending
var fromName = OrderStatus.FromName("Shipped"); // Returns Shipped

// Try parse
if (OrderStatus.TryFromValue(99, out var result))
{
    // result will be null if not found
}

// Get all values
var allStatuses = OrderStatus.GetDeclarations();
foreach (var s in allStatuses)
{
    Console.WriteLine($"{s.Value}: {s.Name}");
}

// Comparison
bool isLater = OrderStatus.Shipped > OrderStatus.Pending; // true
```

#### Advanced Enumeration with Methods

```csharp
public sealed class ShippingMethod : Enumeration<ShippingMethod>
{
    public static readonly ShippingMethod Standard = 
        new(1, "Standard", days: 5, cost: 5.00m);
    public static readonly ShippingMethod Express = 
        new(2, "Express", days: 2, cost: 15.00m);
    public static readonly ShippingMethod Overnight = 
        new(3, "Overnight", days: 1, cost: 25.00m);

    public int DeliveryDays { get; }
    public decimal Cost { get; }

    private ShippingMethod(int value, string name, int days, decimal cost) 
        : base(value, name)
    {
        DeliveryDays = days;
        Cost = cost;
    }

    public DateTime CalculateDeliveryDate(DateTime orderDate)
    {
        return orderDate.AddDays(DeliveryDays);
    }

    public Money GetShippingCost(string currency)
    {
        return Money.Of(Cost, currency);
    }
}

// Usage
var method = ShippingMethod.Express;
var deliveryDate = method.CalculateDeliveryDate(DateTime.Today);
var cost = method.GetShippingCost("USD");
Console.WriteLine($"{method.Name}: Delivers in {method.DeliveryDays} days for {cost}");
```

---

### Rules & Validation

The rule system provides a flexible way to express and enforce domain invariants.

#### Interfaces

```csharp
public interface IRule<TContext>
{
    string Describe(TContext context);
    bool IsViolatedBy(TContext context);
}
```

#### Rule Engine

```csharp
public static class RuleEngine
{
    // Evaluate single rule
    public static RuleEvaluationResult<T> Evaluate<T>(T context, IRule<T> rule);
    
    // Validate single rule (throws on violation)
    public static void Validate<T>(T context, IRule<T> rule);
    
    // Evaluate multiple rules
    public static RuleEvaluationResult<T> Evaluate<T>(T context, 
        IReadOnlyCollection<IRule<T>> rules);
    
    // Validate multiple rules (throws on violation)
    public static void Validate<T>(T context, 
        IReadOnlyCollection<IRule<T>> rules);
}
```

#### Example Rules

```csharp
// Simple rule
public sealed class OrderCannotBeCancelledRule : IRule<Order>
{
    public string Describe(Order order) 
        => "Only pending orders can be cancelled.";
    
    public bool IsViolatedBy(Order order) 
        => order.Status != OrderStatus.Pending;
}

// Rule with dependencies
public sealed class MinimumOrderAmountRule : IRule<Order>
{
    private readonly decimal _minimumAmount;

    public MinimumOrderAmountRule(decimal minimumAmount)
    {
        _minimumAmount = minimumAmount;
    }

    public string Describe(Order order) 
        => $"Order total must be at least {_minimumAmount:C}";
    
    public bool IsViolatedBy(Order order)
    {
        var total = order.Lines.Sum(l => l.Price.Amount);
        return total < _minimumAmount;
    }
}

// Composite rule
public sealed class AllLinesSameCurrencyRule : IRule<Order>
{
    public string Describe(Order order) 
        => "All order lines must use the same currency";
    
    public bool IsViolatedBy(Order order)
    {
        if (order.Lines.Count <= 1) return false;
        
        var firstCurrency = order.Lines.First().Price.Currency;
        return order.Lines.Any(l => l.Price.Currency != firstCurrency);
    }
}
```

#### Usage in Domain Logic

```csharp
public sealed class Order : AggregateRoot<Order, OrderId>
{
    // Constructor validation
    private Order(OrderId id, IEnumerable<OrderLine> lines)
        : base(id, [
            new AtLeastOneLineRule(),
            new AllLinesSameCurrencyRule(),
            new MinimumOrderAmountRule(10.00m)
        ])
    {
        _lines.AddRange(lines);
        // Rules are validated automatically by Validatable base
    }

    // Method validation
    public void ApplyDiscount(decimal percentage)
    {
        var rules = new IRule<Order>[]
        {
            new DiscountPercentageValidRule(percentage),
            new OrderCanHaveDiscountRule()
        };
        
        RuleEngine.Validate(this, rules);
        
        // Apply discount logic
        foreach (var line in _lines)
        {
            line.ApplyDiscount(percentage);
        }
        
        RaiseDomainEvent(new DiscountAppliedEvent(Id, percentage));
    }
}
```

#### Rule Sets

```csharp
public sealed class RuleSet<TContext>
{
    public IReadOnlyCollection<IRule<TContext>> Rules { get; }
    
    public RuleSet(params IRule<TContext>[] rules)
    {
        Rules = rules;
    }
}

// Define reusable rule sets
public static class OrderRuleSets
{
    public static RuleSet<Order> CreationRules => new(
        new AtLeastOneLineRule(),
        new AllLinesSameCurrencyRule(),
        new MinimumOrderAmountRule(10.00m)
    );
    
    public static RuleSet<Order> CancellationRules => new(
        new OrderCannotBeCancelledRule(),
        new NoPartialShipmentsRule()
    );
}

// Usage
RuleEngine.Validate(order, OrderRuleSets.CancellationRules);
```

---

#### Domain Events
Domain events represent something meaningful that happened in the domain.

#### Interface

```csharp
public interface IDomainEvent
{
    // Marker interface - extend with your own properties as needed
}
```

#### Example Events

```csharp
public sealed record OrderCreatedEvent(
    OrderId OrderId,
    CustomerId CustomerId,
    DateTime OccurredOn) : IDomainEvent;

public sealed record OrderShippedEvent(
    OrderId OrderId,
    ShippingAddress Address,
    DateTime ShippedAt) : IDomainEvent;

public sealed record OrderCancelledEvent(
    OrderId OrderId,
    string Reason,
    DateTime CancelledAt) : IDomainEvent;
```

#### Raising Events

```csharp
public sealed class Order : AggregateRoot<Order, OrderId>
{
    public static Order Create(OrderId id, CustomerId customerId, 
                               IEnumerable<OrderLine> lines)
    {
        var order = new Order(id, customerId, lines);
        order.RaiseDomainEvent(new OrderCreatedEvent(
            id, 
            customerId,
            DateTime.UtcNow));
        return order;
    }

    public void Ship(ShippingAddress address)
    {
        RuleEngine.Validate(this, new OrderCanBeShippedRule());
        
        Status = OrderStatus.Shipped;
        ShippedAt = DateTime.UtcNow;
        
        RaiseDomainEvent(new OrderShippedEvent(
            Id, 
            address, 
            ShippedAt.Value));
    }
}
```

#### Dispatching Events

```csharp
// In your application layer / use case handler
public class PlaceOrderHandler
{
    private readonly IOrderRepository _repository;
    // This is not included use your own dispatcher
    private readonly IDomainEventDispatcher _eventDispatcher;

    public async Task<OrderId> HandleAsync(PlaceOrderCommand command)
    {
        var order = Order.Create(
            OrderId.New(),
            command.CustomerId,
            command.Lines);

        await _repository.AddAsync(order);
        
        // Dispatch events after successful persistence
        var events = order.DequeueDomainEvents();
        await _repository.SaveChangesAsync();
        
        foreach (var evt in events)
        {
            await _eventDispatcher.DispatchAsync(evt);
        }

        return order.Id;
    }
}
```

---

## 💡 Design Principles

### Aggregate Design Guidelines

1. **Keep aggregates small**: Only include entities that must change together
2. **Reference by ID**: Reference other aggregates by ID, not direct reference
3. **Enforce invariants**: Use rules to enforce business invariants at boundaries
4. **Use domain events**: Communicate changes to other aggregates via events
5. **One repository per aggregate**: Each aggregate root gets its own repository

### Value Object Guidelines

1. **Immutability**: All value objects should be immutable
2. **Self-validation**: Validate in the constructor or factory method
3. **Meaningful operations**: Provide domain-specific operations (e.g., Money + Money)
4. **No identity**: Value objects are compared by their attributes
5. **Replace, don't modify**: Create new instances rather than modifying existing ones

### Entity Guidelines

1. **Identity is key**: Entities are defined by their identity, not attributes
2. **Protect invariants**: Use private setters and public methods for changes
3. **Validate state transitions**: Enforce rules when changing state
4. **Encapsulate collections**: Expose read-only views of internal collections
5. **Use meaningful names**: Method names should reveal intent (e.g., `Cancel()` not `SetStatus()`)

### Rule Guidelines

1. **Single responsibility**: One rule = one invariant
2. **Clear messages**: Provide clear, user-friendly error messages
3. **Stateless**: Rules should be stateless and reusable
4. **Composable**: Design rules to be composable
5. **Testable**: Keep rules simple and easy to unit test

---

## 📚 API Reference

### Core Types

| Type | Purpose | Key Methods |
|------|---------|-------------|
| `Entity<TDerived, TId>` | Base class for entities | `IsTransient`, equality operators |
| `AggregateRoot<TDerived, TId>` | Base for aggregate roots | `RaiseDomainEvent()`, `DequeueDomainEvents()` |
| `ValueObject<TDerived>` | Base for value objects | `GetEqualityComponents()` |
| `StronglyTypedId<TValue>` | Base for strongly-typed IDs | `IsDefault()`, `Value` |
| `Enumeration<TEnumeration>` | Base for smart enums | `FromValue()`, `FromName()`, `GetDeclarations()` |

### Validation Types

| Type | Purpose | Key Methods |
|------|---------|-------------|
| `IRule<TContext>` | Interface for rules | `IsViolatedBy()`, `Describe()` |
| `RuleEngine` | Static rule evaluator | `Validate()`, `Evaluate()` |
| `RuleSet<TContext>` | Collection of rules | `Rules` |
| `Validatable<TDerived>` | Base for validated types | `Validate()` |

### Interfaces

| Interface | Purpose |
|-----------|---------|
| `IEntity` | Non-generic entity marker |
| `IEntity<TId>` | Generic entity with ID |
| `IAggregateRoot` | Non-generic aggregate marker |
| `IAggregateRoot<TId>` | Generic aggregate with ID |
| `IStronglyTypedId` | Strongly-typed ID marker |
| `IDomainEvent` | Domain event marker |
| `IAuditable` | Auditable entity marker |

---

## 🛠️ Best Practices

### 1. Factory Methods Over Constructors

```csharp
// ✅ Good: Factory method reveals intent
public static Order Create(OrderId id, CustomerId customerId, 
                          IEnumerable<OrderLine> lines)
{
    return new Order(id, customerId, lines);
}

// ❌ Bad: Public constructor exposes implementation
public Order(OrderId id, CustomerId customerId, IEnumerable<OrderLine> lines)
{
    // ...
}
```

### 2. Encapsulate Collections

```csharp
// ✅ Good: Private list, read-only exposure
private readonly List<OrderLine> _lines = [];
public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();

public void AddLine(OrderLine line)
{
    RuleEngine.Validate(line, new ValidOrderLineRule());
    _lines.Add(line);
}

// ❌ Bad: Mutable collection
public List<OrderLine> Lines { get; set; }
```

### 3. Intention-Revealing Methods

```csharp
// ✅ Good: Clear intent
public void Cancel() { ... }
public void Ship(ShippingAddress address) { ... }

// ❌ Bad: Generic setters
public void SetStatus(OrderStatus status) { ... }
public void UpdateAddress(ShippingAddress address) { ... }
```

### 4. Guard Clauses & Validation

```csharp
// ✅ Good: Validate at boundaries
public void ApplyDiscount(decimal percentage)
{
    if (percentage < 0 || percentage > 100)
        throw new ArgumentOutOfRangeException(nameof(percentage));
    
    RuleEngine.Validate(this, new OrderCanHaveDiscountRule());
    
    // Apply discount
}

// ❌ Bad: No validation
public void ApplyDiscount(decimal percentage)
{
    Discount = percentage;
}
```

### 5. Use Domain Events for Side Effects

```csharp
// ✅ Good: Raise event, handle separately
public void Complete()
{
    Status = OrderStatus.Completed;
    CompletedAt = DateTime.UtcNow;
    RaiseDomainEvent(new OrderCompletedEvent(Id, CompletedAt.Value));
    // Email notification handled by event handler
}

// ❌ Bad: Direct coupling
public void Complete(IEmailService emailService)
{
    Status = OrderStatus.Completed;
    emailService.SendOrderCompletedEmail(this); // Coupling!
}
```

---

## 🧪 Testing Examples

### Testing Entities

```csharp
[Fact]
public void Order_Cancel_ShouldChangeStatusToCancelled()
{
    // Arrange
    var order = Order.Create(OrderId.New(), customerId, lines);
    
    // Act
    order.Cancel();
    
    // Assert
    order.Status.Should().Be(OrderStatus.Cancelled);
}

[Fact]
public void Order_Cancel_WhenAlreadyShipped_ShouldThrowException()
{
    // Arrange
    var order = Order.Create(OrderId.New(), customerId, lines);
    order.Ship(address);
    
    // Act & Assert
    var act = () => order.Cancel();
    act.Should().Throw<RuleViolationException<Order>>();
}
```

### Testing Value Objects

```csharp
[Fact]
public void Money_Add_ShouldReturnCorrectSum()
{
    // Arrange
    var money1 = Money.Of(10.00m, "USD");
    var money2 = Money.Of(5.00m, "USD");
    
    // Act
    var result = money1 + money2;
    
    // Assert
    result.Amount.Should().Be(15.00m);
    result.Currency.Should().Be("USD");
}

[Fact]
public void Money_Equality_ShouldBeStructural()
{
    // Arrange
    var money1 = Money.Of(10.00m, "USD");
    var money2 = Money.Of(10.00m, "USD");
    
    // Assert
    money1.Should().Be(money2);
    (money1 == money2).Should().BeTrue();
}
```

### Testing Rules

```csharp
[Fact]
public void OrderCannotBeCancelledRule_WhenPending_ShouldNotBeViolated()
{
    // Arrange
    var order = Order.Create(OrderId.New(), customerId, lines);
    var rule = new OrderCannotBeCancelledRule();
    
    // Act
    var isViolated = rule.IsViolatedBy(order);
    
    // Assert
    isViolated.Should().BeFalse();
}

[Fact]
public void OrderCannotBeCancelledRule_WhenShipped_ShouldBeViolated()
{
    // Arrange
    var order = Order.Create(OrderId.New(), customerId, lines);
    order.Ship(address);
    var rule = new OrderCannotBeCancelledRule();
    
    // Act
    var isViolated = rule.IsViolatedBy(order);
    var description = rule.Describe(order);
    
    // Assert
    isViolated.Should().BeTrue();
    description.Should().NotBeNullOrEmpty();
}
```

---

## 🙏 Acknowledgments

Inspired by:
- Eric Evans - Domain-Driven Design
- Vaughn Vernon - Implementing Domain-Driven Design
- Vladimir Khorikov - DDD in Practice

---

## Support

For questions, issues, or suggestions:
- Create an issue in the repository
- Review existing documentation
- Check sample code in the `/Samples` folder

---
