
## 🏗️ Example DSL: A Financial Strategy DSL

A classic and excellent example of a DSL in F\# is one for **defining and testing financial trading strategies** (often called a Quantitative or Algorithmic Trading DSL).

### **Goal of the DSL**

To allow a user to write a complex trading rule in a way that is clear, sequential, and closely mirrors how a human quant would describe it.

### **F\# Implementation Features Used**

1.  **Computation Expression:** To create a custom flow for strategy steps.
2.  **Custom Infix Operators:** To create clear conditional logic.
3.  **Function Piping (`|>`)**: To sequence market data analysis.

### **Example Syntax**

Here is what the resulting F\# DSL code might look like:

```fsharp
// Define a 'strategy' Computation Expression
let strategy = new StrategyBuilder()

// The DSL in action:
let mySimpleStrategy = strategy {
    // Stage 1: Define Entry Condition
    let! stock = getMarketData "MSFT" |> onTimeframe Daily

    let price = stock.ClosingPrice

    // Use a custom infix operator for readability
    if price > (movingAverage 20 price) **AND** (stochasticOscillator 14) < 20.0 then
        // Stage 2: Execute Trade
        buy 1000 shares "MSFT"

    // Stage 3: Define Exit Condition
    when price < (movingAverage 50 price) **OR** (timeInMarket > 5 days) then
        sellAll "MSFT"
}
```

In this example, keywords like `strategy`, `let!`, `buy`, `when`, `sellAll`, and the `**AND**` operator are all part of the custom DSL, giving a non-programmer (like a financial analyst) a highly readable and domain-specific way to express complex logic.