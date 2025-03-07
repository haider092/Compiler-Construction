using System;
using System.Collections.Generic;

class Program
{
    // Grammar example: E -> T E' | E' -> + T E' | epsilon | T -> ( E ) | id
    private Dictionary<string, List<List<string>>> grammar = new Dictionary<string, List<List<string>>>();
    private Dictionary<string, HashSet<string>> firstSets = new Dictionary<string, HashSet<string>>();

    public Program()
    {
        // Define the grammar
        grammar["E"] = new List<List<string>>() { new List<string> { "T", "E'" } };
        grammar["E'"] = new List<List<string>>() { new List<string> { "+", "T", "E'" }, new List<string> { "epsilon" } };
        grammar["T"] = new List<List<string>>() { new List<string> { "(", "E", ")" }, new List<string> { "id" } };

        // Initialize FIRST sets
        foreach (var nonTerminal in grammar.Keys)
        {
            firstSets[nonTerminal] = new HashSet<string>();
        }
    }

    // Simple computation of FIRST sets
    public void ComputeFirstSets()
    {
        bool changed = true;
        while (changed)
        {
            changed = false;
            foreach (var nonTerminal in grammar.Keys)
            {
                foreach (var production in grammar[nonTerminal])
                {
                    foreach (var symbol in production)
                    {
                        // If the symbol is a terminal, add it to FIRST set
                        if (IsTerminal(symbol))
                        {
                            if (firstSets[nonTerminal].Add(symbol))
                            {
                                changed = true;
                            }
                            break; // Terminals stop the search, move to the next production
                        }
                        else
                        {
                            // If the symbol is a non-terminal, add its FIRST set to the current non-terminal's FIRST set
                            int before = firstSets[nonTerminal].Count;
                            firstSets[nonTerminal].UnionWith(firstSets[symbol]);

                            // If the non-terminal can derive epsilon, continue checking the next symbols
                            if (!firstSets[symbol].Contains("epsilon"))
                                break;

                            // If we finish the entire production and it can derive epsilon, add epsilon to FIRST set
                            if (firstSets[nonTerminal].Count > before)
                            {
                                changed = true;
                            }
                        }
                    }
                }
            }
        }
    }

    // Check if a symbol is a terminal (assuming terminals are lowercase and non-terminals are uppercase)
    private bool IsTerminal(string symbol)
    {
        return symbol.ToLower() == symbol || symbol == "epsilon";
    }

    // Display the FIRST sets
    public void DisplayFirstSets()
    {
        Console.WriteLine("FIRST sets:");
        foreach (var entry in firstSets)
        {
            Console.WriteLine($"{entry.Key}: {{ {string.Join(", ", entry.Value)} }}");
        }
    }

    // Main method to demonstrate the process
    public static void Main(string[] args)
    {
        var parser = new Program();

        parser.ComputeFirstSets();
        parser.DisplayFirstSets();
    }
}
