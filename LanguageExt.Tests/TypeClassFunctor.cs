using Xunit;
using System.Linq;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using LanguageExt;
using System;

namespace LanguageExt.Tests
{
    public class TypeClassFunctor
    {
        [Fact]
        public void Tuple2FirstItemMap()
        {
            var tup = Tuple(42, "Paul");

            tup = FTupleFst<int, string, int>.Inst.Map(tup, x => x * 2);

            Assert.True(tup.Item1 == 84);
            Assert.True(tup.Item2 == "Paul");
        }

        [Fact]
        public void Tuple2SecondItemMap()
        {
            var tup = Tuple(42, "Paul");

            tup = FTupleSnd<int, string, string>.Inst.Map(tup, x => x + " Louth");

            Assert.True(tup.Item1 == 42);
            Assert.True(tup.Item2 == "Paul Louth");
        }

        [Fact]
        public void Tuple3FirstItemMap()
        {
            var tup = Tuple(42, "Paul", true);

            tup = FTupleFst<int, string, bool, int>.Inst.Map(tup, x => x * 2);

            Assert.True(tup.Item1 == 84);
            Assert.True(tup.Item2 == "Paul");
            Assert.True(tup.Item3 == true);
        }

        [Fact]
        public void Tuple3SecondItemMap()
        {
            var tup = Tuple(42, "Paul", true);

            tup = FTupleSnd<int, string, bool, string>.Inst.Map(tup, x => x + " Louth");

            Assert.True(tup.Item1 == 42);
            Assert.True(tup.Item2 == "Paul Louth");
            Assert.True(tup.Item3 == true);
        }

        [Fact]
        public void Tuple3ThirdItemMap()
        {
            var tup = Tuple(42, "Paul", true);

            tup = FTupleThrd<int, string, bool, bool>.Inst.Map(tup, _ => false);

            Assert.True(tup.Item1 == 42);
            Assert.True(tup.Item2 == "Paul");
            Assert.True(tup.Item3 == false);
        }

        [Fact]
        public void ListProject()
        {
            var list = List("100", "50", "25");
            var opt = Some("100");

            // Parse a list of strings into a list of ints
            Lst<int> res1 = ParseInts<FLst<string, int>, Lst<string>, Lst<int>>(list);

            // Parse an option of string into an option of int
            Option<int> res2 = ParseInts<FOption<string, int>, Option<string>, Option<int>>(opt);

            Assert.True(res1[0] == 100);
            Assert.True(res1[1] == 50);
            Assert.True(res1[2] == 25);
            Assert.True(res2 == 100);
        }

        /// Generic usage
        public static FB ParseInts<Functor, FA, FB>(FA input)
            where Functor : struct, Functor<FA, FB, string, int> => 
            default(Functor).Map(input, Int32.Parse);

    }
}
