using System;
using System.Linq;
using BettingEngine.Betting;

namespace BettingEngine.Example
{
    public class FootballBetting
    {
        private readonly SingleChoiceBet _bet;

        public FootballBetting()
        {
            _bet = new SingleChoiceBet(PossibleResults.AllWithDescription.Select(_ => _.Result));

            Console.WriteLine("Bet has been created.");
            Console.WriteLine();
        }

        public void OfferWagers()
        {
            while (true)
            {
                PrintOdds();

                // Get wager's expected results.
                IResult expectedResult;
                Console.WriteLine("Select a result to place a bet: ");
                while (true)
                {
                    Console.Write("> ");
                    try
                    {
                        var index = int.Parse(Console.ReadLine()?.TrimEnd('\r', '\n'));
                        expectedResult = PossibleResults.AllWithDescription[index - 1].Result;
                        break;
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Unknown format of input. Use 1, 2 or 3 to select a result.");
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine("Unknown index. Use indices 1, 2 or 3 to select a result.");
                    }
                }

                // Get wager's stake value.
                decimal stakeValue;
                Console.WriteLine("Specify stake value: ");
                while (true)
                {
                    Console.Write("> ");
                    try
                    {
                        stakeValue = decimal.Parse(Console.ReadLine()?.TrimEnd('\r', '\n'));
                        break;
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine($"Unknown format of input. Specify a decimal value (e.g. '{42M:F2}').");
                    }
                }

                _bet.AddExpectedResults(expectedResult, stakeValue);
                Console.WriteLine("Wager has been created.");
                Console.WriteLine();
            }
        }

        private void PrintOdds()
        {
            Console.WriteLine("Odds:");
            var maximumDescriptionLength = PossibleResults.AllWithDescription.Select(_ => _.Description.Length).Max();
            var index = 0;
            foreach (var (result, description) in PossibleResults.AllWithDescription)
            {
                var odds = _bet.GetOdds(result, result);
                Console.WriteLine(
                    $"[{index + 1}] {description.PadRight(maximumDescriptionLength)} | ${odds:F2} for ${1M:F2}");
                ++index;
            }
        }

        public static void Main()
        {
            new FootballBetting().OfferWagers();
        }

        public static class PossibleResults
        {
            public static readonly IResult HomeTeamWins = new Result();
            public static readonly IResult VisitingTeamWins = new Result();
            public static readonly IResult Draw = new Result();

            public static (IResult Result, string Description)[] AllWithDescription => new[]
            {
                (HomeTeamWins, "Home Team Wins"),
                (VisitingTeamWins, "Visitor Team Wins"),
                (Draw, "Draw")
            };
        }
    }
}