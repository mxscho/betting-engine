# BettingEngine

This .NET Standard library can be used to calculate odds or outcomes of
[parimutuel bets](https://en.wikipedia.org/wiki/Parimutuel_betting).
Therefore, it works like a *totalizator* for different types of bets,
capable of displaying odds and outcomes dynamically after each wager in `O(n)`
where `n` is the number of possible results of a bet.

Bets work like this: All wagers are placed together in a pool.
Odds and winnings are calculated by sharing the pool and thus the
stakes of all losing wagers among all winning wagers with regard to their stakes.
This also means that winning wagers always get their respective stake back first
regardless of the pool sharing calculation.

## Terminology

- **Bet**: The whole game of predicting which results will "happen",
    defined for a specific set of results.
- **Wager**: Participation at one single bet,
    specifying expected results and a stake.
- **Odds**: Amount of winnings for a specific expected result in case of a specific
    actual result happening, relative to the stake.
    (Example: Odds being 4.2 means that for each $1 of the winner wager's stake,
    the winner wager's owner will get $4.20 of the shared pool.)
- **Outcome**: Amount of winnings a wager gets from the pool of one bet in case of a win.
    This is basically the odds of a wager's expected results times the wager's stake.

### Single-Choice Bets

Single-choice bets are bets with a specified set of possible results,
where only one of these results can "happen" as the actual result.
Wagers specify exactly one of these possible results as the expected one.
Therefore, the winning wagers are the ones which chose exactly the single actual result
as the expected one.

The stake of all wagers which did not choose this single actual result as the expected one
will be shared among the winning wagers.

#### Example

- **Results:** {`A`, `B`, `C`}
- **Possible Results:** `A`, `B`, `C`

If `A` is the actual result, the stakes of the wagers betting on `B` and `C`
will be shared among all wagers betting on `A`.

Single-choice bets are implemented as a specialization of multiple-choice bets
which are described in the following part.

### Multiple-Choice Bets 

Multiple-choice bets are bets with a specified set of possible results,
where the actual results that "happen" can be any subset of these possible results.
Wagers specify a specific subset of these possible results as the "expected ones".
To be considered a winner, a wager only has to include one of the actual results in its
expected results set.
This means that a wager is winning if the intersection of its expected results set
and the actual results set includes at least one result.

However, not all winning wagers receive the same amount of winnings.
Just like with the single-choice bets, stakes of losing wagers are shared among
all winning wagers.
The difference is that in this case, the winning wagers only get the stakes of
losing wagers with which they do not share expected results.
This means that a winning wager can only claim a losing wager's stake if the
intersection of their respective expected result sets does not include any results.
In the special case where none of the existing winning wagers can claim a losing wager
because of overlapping expected result sets, the losing wager's stake is shared
among all existing winning wagers.

This way, wagers including less results in their expected result set usually have
a lower chance of winning and therefore higher odds and a higher potential outcome.

#### Example

- **Results:** {`A`, `B`, `C`}
- **Possible Results:** {`A`}, {`B`}, {`C`}, {`A`, `B`}, {`A`, `C`}, {`B`, `C`}

If {`A`} is the actual result, the stakes of wagers are shared in this way:

- Wagers expecting {`A`} get a share of the stakes of wagers expecting
    {`B`}, {`C`} or {`B`, `C`}.
- Wagers expecting {`A`, `B`} get a share of the stakes of wagers expecting
    {`C`}.
- Wagers expecting {`A`, `C`} get a share of the stakes of wagers expecting
    {`B`}.

If {`A`, `B`} is the actual result, the stakes of wagers are shared in this way:

- Wagers expecting {`A`} get a share of the stakes of wagers expecting
    {`C`}.
- Wagers expecting {`A`, `B`} get a share of the stakes of wagers expecting
    {`C`}.
- Wagers expecting {`A`, `C`} get no additional winnings. They do (as everyone else)
    get back their own stake of course.

## Example

This is an example for a single-choice bet with 3 possible results `A`, `B` and `C`:

```c#
var resultA = new Result();
var resultB = new Result();
var resultC = new Result();

// Create new bet.
var bet = new SingleChoiceBet(new[]
{
    resultA,
    resultB,
    resultC
});

// Add wagers by specifying expected result and stake.
var wagerB = bet.AddExpectedResults(resultB, 42.00M);
var wagerC = bet.AddExpectedResults(resultC, 13.37M);

// At any time, get odds for an expected result in case of an actual result.
// For multiple-choice bets, these two values (result sets) may be different
// but the resulting odds are still greater than zero.
var oddsA = bet.GetOdds(resultA, resultA);

// Add more wagers.
// var wager = ...

// After all wagers have been added, get the outcome of the bet for a specific wager.
var actualResult = resultA;
var outcome = bet.GetOutcome(wagerA, actualResult);
```

For multiple-choice bets, instead of handing single `IResult` instances to
`AddExpectedResults(...)`, `GetOdds(...)` or `GetOutcome(...)`, use an `IResultSet`
instance to specify a set of `IResult` instances.

Another working example can be found in the `BettingEngine.Betting.Example` assembly.
It offers wagers for a football game via console inputs and displays the current odds
after each added wager. 
