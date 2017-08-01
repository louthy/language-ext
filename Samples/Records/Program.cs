using LanguageExt;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Records
{
    class Program
    {
        static void Main(string[] args)
        {
            var opt1 = Maybe.Just(100);
            var opt2 = Maybe.Nothing<int>();

            var res1 = opt1.Match(
                           Just: x => x * 2,
                           Nothing: () => 0);

            var res2 = opt2.Match(
                           Just: x => x * 2,
                           Nothing: () => 0);

            var eqA = opt1 == opt2;
            var eqB = opt1 != opt2;

            Console.WriteLine($"opt1: {opt1}");
            Console.WriteLine($"opt2: {opt2}");
            Console.WriteLine($"res1: {res1}");
            Console.WriteLine($"res2: {res2}");
            Console.ReadKey();
        }
    }
}
