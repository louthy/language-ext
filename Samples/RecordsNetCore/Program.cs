using LanguageExt;
using static LanguageExt.Prelude;
using System;

namespace RecordsNetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            MaybeRecordTypeExample();
        }

        private static void MaybeRecordTypeExample()
        {
            var opt1 = Maybe.Just(100);
            var opt2 = Maybe.Just(100);
            var optN = Maybe.Nothing<int>();

            var res1 = opt1.Match(
                           Just: x => x * 2,
                           Nothing: () => 0);

            var res2 = optN.Match(
                           Just: x => x * 2,
                           Nothing: () => 0);

            var sign = opt1.Equals(optN) ? "==" : "!=";
            Console.WriteLine($"{opt1} {sign} {optN}");

            sign = opt1.Equals(opt2) ? "==" : "!=";
            Console.WriteLine($"{opt1} {sign} {opt2}");

            Console.WriteLine($"opt1: {opt1}");
            Console.WriteLine($"optN: {optN}");
            Console.WriteLine($"res1: {res1}");
            Console.WriteLine($"res2: {res2}");
            Console.ReadKey();
        }
    }
}