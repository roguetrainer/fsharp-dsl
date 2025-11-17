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