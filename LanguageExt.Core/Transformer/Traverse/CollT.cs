#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace LanguageExt
{
    internal class CollT
    {
        public static List<List<B>> AllCombinationsOf<A, B>(List<A>[] sets, Func<A, B> f)
        {
            var combinations = new List<List<B>>();
            if (sets.Length == 0) return combinations;

            foreach (var value in sets[0])
            {
                combinations.Add(new List<B> {f(value)});
            }

            foreach (var set in sets.Skip(1))
            {
                combinations = AddExtraSet<A, B>(combinations, set, f);
            }

            return combinations;
        }

        static List<List<B>> AddExtraSet<A, B>(List<List<B>> combinations, List<A> set, Func<A, B> f)
        {
            var newCombinations = 
                from combination in combinations
                from value in set
                select new List<B>(combination) { f(value) };

            return newCombinations.ToList();
        }
    }
}
