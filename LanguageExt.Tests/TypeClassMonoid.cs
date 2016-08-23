using Xunit;
using System.Linq;
using LanguageExt.TypeClasses;
using LanguageExt.Instances;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using LanguageExt;

namespace LanguageExtTests
{
    public class TypeClassMonoid
    {
        [Fact]
        public void IntMonoid()
        {
            var res = mconcat<TInt, int>(1, 2, 3, 4, 5);

            Assert.True(res == 15);
        }

        [Fact]
        public void ListMonoid()
        {
            var res = mconcat<TLst<int>, Lst<int>>(List(1, 2, 3), List(4, 5));

            Assert.True(res.Sum() == 15 && res.Count == 5);
        }

        [Fact]
        public void StringMonoid()
        {
            var res = mconcat<TString, string>("mary ", "had ", "a ", "little ", "lamb");

            Assert.True(res == "mary had a little lamb");
        }
    }
}
