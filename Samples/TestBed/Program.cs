////////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                    //
//                                                                                                    //
//     NOTE: This is just my scratch pad for quickly testing stuff, not for human consumption         //
//                                                                                                    //
//                                                                                                    //
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Reflection;
using LanguageExt;
using LanguageExt.Common;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq.Expressions;
using TestBed;

class Program
{
    static void Main(string[] args)
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                                                                    //
        //                                                                                                    //
        //     NOTE: This is just my scratch pad for quickly testing stuff, not for human consumption         //
        //                                                                                                    //
        //                                                                                                    //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        TraverseTest();
    }

    static void TraverseTest()
    {
        var xxs = Seq(Seq(1, 2, 3, 4), Seq(10, 20, 30), Seq(100, 200));

        // var size = xxs.Fold(1, (s, xs) => s * xs.Count);
        // var blocks = new int[][size];
        // for(var ix = 0; ix < blocks.Length; ix++)
        // {
        //     blocks[ix] = new int[xxs.Count];
        // }
        //

        var combs = AllCombinationsOf(xxs.Map(xs => new List<int>(xs)).ToArray())
                        .Map(Seq)
                        .ToSeq();

        /*
        var xxs2 = xxs.FoldBack(Seq<Seq<int>>(), 
            (ss, xs) => 
            */

        // var xxs2 = xxs.FoldBack(Seq<Seq<int>>(),
        //     (ss, xs) => xs.FoldBack(ss.Add(Seq<int>()), 
        //         (s, x) => s.Head.Add(x).Cons(s.Tail)));
    }

    static List<List<A>> AllCombinationsOf<A>(params List<A>[] sets)
    {
        // need array bounds checking etc for production
        var combinations = new List<List<A>>();
        if (sets.Length == 0) return combinations;

        // prime the data
        foreach (var value in sets[0])
        {
            combinations.Add(new List<A> {value});
        }

        foreach (var set in sets.Skip(1))
        {
            combinations = AddExtraSet(combinations, set);
        }

        return combinations;
    }

    static List<List<A>> AddExtraSet<A>
        (List<List<A>> combinations, List<A> set)
    {
        var newCombinations = 
            from combination in combinations
            from value in set
            select new List<A>(combination) { value };

        return newCombinations.ToList();
    }
}
