// -----------------------------------------------------------------------------
// 1. Defining the Domain-Specific Types (The Abstract Syntax Tree - AST)
// -----------------------------------------------------------------------------

/// The Instruction type is a Discriminated Union (DU) that models
/// the basic, fundamental actions a financial strategy can take.
/// This acts as the AST (Abstract Syntax Tree) for our DSL.
type TradeInstruction =
    | Buy of string * decimal // Symbol and Quantity
    | Sell of string * decimal // Symbol and Quantity
    | Wait of int // Wait for a number of seconds
    | Log of string // Log a message for debugging/auditing
    | Sequence of TradeInstruction list // Grouping of instructions

/// The type produced by the builder: a sequence of instructions.
type Strategy<'T> = TradeInstruction list

// -----------------------------------------------------------------------------
// 2. The StrategyBuilder Class (The Computation Expression Builder)
// -----------------------------------------------------------------------------

/// This class defines the behavior of the 'strategy { ... }' computation expression.
/// It uses a method for each F# keyword we want to customize (like 'return', 'let!', 'yield').
type StrategyBuilder() =

    // --- Core DSL Construction ---

    /// This method is called when 'return <value>' is used inside the block.
    /// In this specific DSL, we enforce that the 'return' value must be Unit,
    /// because the result of a strategy is always the built list of instructions.
    member _.Return(_: unit) : Strategy<unit> =
        []

    /// This method is called when 'return! <value>' is used, allowing us to
    /// flatten an existing strategy into the current one.
    member _.ReturnFrom (instructions: Strategy<'T>) : Strategy<'T> =
        instructions

    /// This method is called to sequence two instructions/computations.
    /// The 'm' parameter is the result of the previous computation (the list of instructions built so far).
    /// The 'f' parameter is the next computation step (a function).
    /// Since we are building a simple sequential list, we don't need the 'f' function;
    /// we just return the instructions we've collected so far, as they will be
    /// appended via the 'Combine' method.
    member _.Bind (m: Strategy<'T>, f: 'T -> Strategy<'U>) : Strategy<'U> =
        // For a side-effect DSL like this, Bind is often implemented with Combine.
        // However, a simple Bind implementation for list collection can look like this:
        m

    /// Called to combine the result of two sequential computations.
    /// This is where the magic of DSL creation happens, as it stitches instructions together.
    member _.Combine (m1: Strategy<'T>, m2: Strategy<'U>) : Strategy<'U> =
        m1 @ m2 // Append the second list of instructions to the first.

    /// Called for simple expressions without explicit keywords (e.g., just 'Buy ...' on a line).
    /// Since our DSL doesn't implicitly wrap expressions, we use 'Combine' logic here.
    member _.Zero() : Strategy<unit> =
        []

    /// Called when the 'yield' keyword is used.
    /// This allows the developer to explicitly generate a single instruction.
    member _.Yield (instruction: TradeInstruction) : Strategy<unit> =
        [instruction]

    // --- Custom DSL Operations (The actual keywords the user types) ---

    // The [<CustomOperation("...")>] attribute lets us define a new keyword
    // that the user can use inside the 'strategy { ... }' block.

    /// The 'buy' keyword: Adds a Buy instruction to the strategy.
    /// It must take the current state (the accumulated instructions) and the new parameters.
    [<CustomOperation("buy")>]
    member _.Buy (instructions: Strategy<'T>, symbol: string, quantity: decimal) : Strategy<'T> =
        instructions @ [Buy (symbol, quantity)]

    /// The 'sell' keyword: Adds a Sell instruction to the strategy.
    [<CustomOperation("sell")>]
    member _.Sell (instructions: Strategy<'T>, symbol: string, quantity: decimal) : Strategy<'T> =
        instructions @ [Sell (symbol, quantity)]

    /// The 'wait' keyword: Adds a Wait instruction.
    [<CustomOperation("wait")>]
    member _.Wait (instructions: Strategy<'T>, seconds: int) : Strategy<'T> =
        instructions @ [Wait seconds]

    /// The 'log' keyword: Adds a Log instruction.
    [<CustomOperation("log")>]
    member _.Log (instructions: Strategy<'T>, message: string) : Strategy<'T> =
        instructions @ [Log message]

// 3. The Builder Instance

/// The public instance that acts as the keyword for the strategy DSL block.
let strategy = new StrategyBuilder()

// -----------------------------------------------------------------------------
// 4. Example Usage of the DSL
// -----------------------------------------------------------------------------

/// Example of defining a strategy using the custom DSL syntax.
let simpleTrendStrategy = strategy {
    // These look like imperative commands, but they are compiled into
    // sequential calls to the StrategyBuilder's custom methods.

    log "Starting simple trend following strategy..."
    buy "MSFT" 100M
    wait 60 // Wait 60 seconds for the trade to settle
    log "Initial position established."

    // Use F# control flow (if/then/else) which the builder naturally supports
    let price = 150.0M // Mock price check
    if price > 140.0M then
        log "Price is above threshold. Scaling in."
        buy "MSFT" 50M
    else
        log "Price is low. Holding position."

    // Explicitly yield the AST instructions (useful for complex logic)
    yield Log "Ending execution phase."
    sell "MSFT" 150M

    // The 'return ()' is not strictly necessary but is good practice to show
    // the block returns a Strategy<unit> (a list of instructions).
    return ()
}

// -----------------------------------------------------------------------------
// 5. Interpreter (Processing the DSL)
// -----------------------------------------------------------------------------

/// A function to execute (interpret) the generated Strategy AST.
let rec executeInstruction instruction =
    match instruction with
    | Buy (symbol, quantity) ->
        printfn "[TRADE] Executing BUY %M shares of %s." quantity symbol
    | Sell (symbol, quantity) ->
        printfn "[TRADE] Executing SELL %M shares of %s." quantity symbol
    | Wait seconds ->
        printfn "[SYSTEM] Waiting for %d seconds." seconds
    | Log message ->
        printfn "[AUDIT] %s" message
    | Sequence instructions ->
        instructions |> List.iter executeInstruction

/// Executes a full strategy (the list of instructions)
let executeStrategy (instructions: Strategy<'T>) =
    printfn "\n--- Strategy Execution Log ---"
    instructions |> List.iter executeInstruction
    printfn "--- Execution Complete ---\n"


// -----------------------------------------------------------------------------
// Execution Point
// -----------------------------------------------------------------------------
// The code below runs the example strategy and prints the result to the console.
executeStrategy simpleTrendStrategy