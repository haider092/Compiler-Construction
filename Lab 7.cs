using System;
using System.Collections.Generic;

class Program
{
    // Grammar: 
    // S → A B | C D
    // A → a | ε
    // B → b
    // C → c
    // D → d
    private Dictionary<string, List<List<string>>> grammar = new Dictionary<string, List<List<string>>>();
    private Dictionary<string, HashSet<string>> firstSets = new Dictionary<string, HashSet<string>>();
    private Dictionary<string, HashSet<string>> followSets = new Dictionary<string, HashSet<string>>();

    public Program()
    {
        // Define the grammar
        grammar["S"] = new List<List<string>>() { new List<string> { "A", "B" }, new List<string> { "C", "D" } };
        grammar["A"] = new List<List<string>>() { new List<string> { "a" }, new List<string> { "epsilon" } };
        grammar["B"] = new List<List<string>>() { new List<string> { "b" } };
        grammar["C"] = new List<List<string>>() { new List<string> { "c" } };
        grammar["D"] = new List<List<string>>() { new List<string> { "d" } };

        // Initialize FIRST and FOLLOW sets
        foreach (var nonTerminal in grammar.Keys)
        {
            firstSets[nonTerminal] = new HashSet<string>();
            followSets[nonTerminal] = new HashSet<string>();
        }

        // The FOLLOW set of the start symbol contains the end-of-input symbol ($)
        followSets["S"].Add("$");
    }

    // Compute FIRST sets
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
                        // If the symbol is a terminal, add it to the FIRST set of the non-terminal
                        if (IsTerminal(symbol))
                        {
                            if (firstSets[nonTerminal].Add(symbol))
                            {
                                changed = true;
                            }
                            break; // Terminals stop the search for this production
                        }
                        else // If the symbol is a non-terminal
                        {
                            int before = firstSets[nonTerminal].Count;
                            firstSets[nonTerminal].UnionWith(firstSets[symbol]);
                            if (firstSets[nonTerminal].Count > before)
                            {
                                changed = true;
                            }

                            // If the FIRST set of the non-terminal contains epsilon, continue with the next symbol
                            if (firstSets[symbol].Contains("epsilon"))
                            {
                                continue;
                            }
                            break;
                        }
                    }
                }
            }
        }
    }

    // Compute FOLLOW sets
    public void ComputeFollowSets()
    {
        bool changed = true;
        while (changed)
        {
            changed = false;
            foreach (var nonTerminal in grammar.Keys)
            {
                foreach (var production in grammar[nonTerminal])
                {
                    for (int i = 0; i < production.Count; i++)
                    {
                        string currentSymbol = production[i];
                        if (IsNonTerminal(currentSymbol))
                        {
                            if (i + 1 < production.Count)
                            {
                                string nextSymbol = production[i + 1];
                                // If the next symbol is a terminal, add it to the FOLLOW set
                                if (IsTerminal(nextSymbol))
                                {
                                    if (followSets[currentSymbol].Add(nextSymbol))
                                    {
                                        changed = true;
                                    }
                                }
                                else // If the next symbol is a non-terminal, add the FIRST set of the next symbol (except epsilon)
                                {
                                    int before = followSets[currentSymbol].Count;
                                    followSets[currentSymbol].UnionWith(firstSets[nextSymbol]);
                                    followSets[currentSymbol].Remove("epsilon");
                                    if (followSets[currentSymbol].Count > before)
                                    {
                                        changed = true;
                                    }

                                    // If the next symbol can derive epsilon, propagate FOLLOW sets
                                    if (firstSets[nextSymbol].Contains("epsilon"))
                                    {
                                        int beforeFollow = followSets[currentSymbol].Count;
                                        followSets[currentSymbol].UnionWith(followSets[nonTerminal]);
                                        if (followSets[currentSymbol].Count > beforeFollow)
                                        {
                                            changed = true;
                                        }
                                    }
                                }
                            }
                            else // If the current symbol is the last one, propagate the FOLLOW set of the non-terminal
                            {
                                int before = followSets[currentSymbol].Count;
                                followSets[currentSymbol].UnionWith(followSets[nonTerminal]);
                                if (followSets[currentSymbol].Count > before)
                                {
                                    changed = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    // Check if a symbol is a terminal (lowercase or "epsilon")
    private bool IsTerminal(string symbol)
    {
        return symbol.ToLower() == symbol || symbol == "epsilon";
    }

    // Check if a symbol is a non-terminal (uppercase)
    private bool IsNonTerminal(string symbol)
    {
        return symbol.ToUpper() == symbol;
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

    // Display the FOLLOW sets
    public void DisplayFollowSets()
    {
        Console.WriteLine("\nFOLLOW sets:");
        foreach (var entry in followSets)
        {
            Console.WriteLine($"{entry.Key}: {{ {string.Join(", ", entry.Value)} }}");
        }
    }

    // Main method to demonstrate the process
    public static void Main(string[] args)
    {
        var parser = new Program();

        // Compute FIRST and FOLLOW sets
        parser.ComputeFirstSets();
        parser.ComputeFollowSets();

        // Display the results
        parser.DisplayFirstSets();
        parser.DisplayFollowSets();
    }
}
