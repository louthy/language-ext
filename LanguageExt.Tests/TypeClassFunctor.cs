using Xunit;
using System.Linq;
using LanguageExt.TypeClasses;
using LanguageExt.Instances;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using LanguageExt;

namespace LanguageExtTests
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
    }
}
