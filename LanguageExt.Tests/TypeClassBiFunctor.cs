using Xunit;
using System.Linq;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using LanguageExt;

namespace LanguageExt.Tests
{
    public class TypeClassBiFunctor
    {
        [Fact]
        public void Tuple2Map()
        {
            var tup = Tuple(42, "Paul");

            var res = FTupleBi<int, string, string, int>.Inst.BiMap(tup, 
                a => a.ToString(),
                b => b.Length);

            Assert.True(res.Item1 == "42");
            Assert.True(res.Item2 == 4);
        }

        [Fact]
        public void Tuple3Map()
        {
            var tup = Tuple(42, "Paul", true);

            var res = FTupleTri<int, string, bool, string, int, string>.Inst.TriMap(tup,
                a => a.ToString(),
                b => b.Length,
                c => c.ToString()
                );

            Assert.True(res.Item1 == "42");
            Assert.True(res.Item2 == 4);
            Assert.True(res.Item3 == "True");
        }
    }
}
