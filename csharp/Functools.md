- ```Select()``` method is same as Python's ```map()``` function

- Use ```Aggregate``` method (or ```reduce``` in python) to cumulatively do something on the collection (i.e. like `reduce`). For example:
```
private static Dictionary<char, int> NucleotideDict = new Dictionary<char, int>()
        {
            {'A', 0},
            {'C', 0},
            {'G', 0},
            {'T', 10}
        };
	
public static IDictionary<char, int> Count(string sequence) =>
    sequence.Aggregate(NucleotideDict, (dict, letter) =>
		{
			         if (dict.ContainsKey(letter)) dict[letter]++;
         else throw new ArgumentException($"Invalid symbol '{letter}' in \"{sequence}\".");
         return dict;
								     },
				        dict => dict);
```

The ```Count``` method above is supposed to take in a sequence of strings, and return a copy of NucleotideDict, updated with how many times each letter in the Dictionary occurs in the string.

In the first run of aggregation, dict in the above equals NucleotideDict, and performs the operation with the first letter in the sequence, which returns the dict updated. 

Then on the second run, dict in the above then equals the updated dict, and is operated with second letter in sequence, and so on and so forth. 

After all runs are done, the third argument (dict=>dict) specifies what part of the aggregation is to be returned. In this case we return everything.

