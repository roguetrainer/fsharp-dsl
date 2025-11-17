
Here are some of the best and most famous examples of Domain-Specific Languages built using F\#'s Computation Expressions and other features, drawing from the F\# documentation and broader ecosystem:

-----

## Excellent Examples of F\# DSLs

### 1\. The `async` Workflow (The Standard Bearer)

The `async` DSL is the feature most F\# developers encounter first and is the most widely adopted computation expression in the .NET world.

  * **Domain:** Asynchronous and concurrent programming.
  * **Syntax Keyword:** `async`
  * **What it does:** It lets you write complex asynchronous logic as if it were sequential imperative code, abstracting away callback chains and thread management. The `let!` keyword is key here, defining the sequence of asynchronous operations.
  * **Example:**
    ```fsharp
    let fetchStrategyData = async {
        let! apiKey = readKeyFromFile "config.txt"
        let! response = http.get $"api/strategy/{apiKey}"
        return response.body
    }
    ```

### 2\. The `query` Expression (LINQ)

The `query` expression is F\#'s implementation of Language Integrated Query (LINQ), allowing you to write powerful database or collection filtering logic directly in the language.

  * **Domain:** Data access and collection manipulation.
  * **Syntax Keyword:** `query`
  * **What it does:** It translates a high-level, SQL-like syntax (`select`, `where`, `groupBy`) into executable code (either database commands via an ORM or collection methods for in-memory data).
  * **Example:**
    ```fsharp
    let highlyRatedStocks (stockList: list<Stock>) =
        query {
            for stock in stockList do
            where (stock.Rating >= 5)
            sortByDescending stock.MarketCap
            take 10
            select stock.Symbol
        }
    ```

### 3\. The `list` / `array` / `seq` Expressions (Collection Building)

These are simple but powerful computation expressions used to declaratively build collections. They redefine the meaning of `for`, `if`, and `yield` to create data structures.

  * **Domain:** Generating and filtering collections.
  * **Syntax Keyword:** `list`, `array`, `seq` (sequence)
  * **What it does:** It allows you to use imperative-looking loops and conditionals to define the contents of a functional collection without relying on mutable state.
  * **Example:**
    ```fsharp
    let squaredEvens = [
        for i in 1..10 do
        if i % 2 = 0 then
            yield i * i
    ]
    // Result: [4; 16; 36; 64; 100]
    ```

### 4\. Custom Parser DSLs (The Domain Specific)

For complex domain models, developers frequently build custom builders tailored to specific needs:

  * **Configuration & Validation:** A builder might enforce that certain configuration settings *must* be present, or that values *must* be in a specific range, combining configuration reading with immediate validation logic.
  * **Web Routing:** A web framework can use a builder to define routes fluently:
    ```fsharp
    let webRoutes = router {
        get "/home" handleHome
        post "/user" handleUserCreation
        group "/admin" {
            get "/dashboard" handleAdminDashboard
        }
    }
    ```
  * **Unit of Measure Validation:** While not a computation expression, the built-in **Units of Measure** feature is a core F\# language-level DSL that provides compile-time type checking for physical quantities (e.g., preventing you from accidentally adding meters to kilograms).

These examples showcase how F\# uses computation expressions to turn boilerplate or complex logic into simple, readable, and highly **declarative** domain-specific code.