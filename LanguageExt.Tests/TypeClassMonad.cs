using Xunit;
using System.Linq;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using LanguageExt;

namespace LanguageExtTests
{
    public class TypeClassMonad
    {
        [Fact]
        public void MonadReturnTest()
        {
            Option<int> x = DoubleAndLift<MOption<int>, Option<int>, TInt, int>(100);
            Assert.True(x.IfNone(0) == 200);
        }

        public static MA DoubleAndLift<MONAD, MA, NUM, A>(A num)
            where MONAD : struct, Monad<MA, A>
            where NUM   : struct, Num<A> =>
            Return<MONAD, MA, A>(product<NUM, A>(num, fromInteger<NUM, A>(2)));

        public static MA DoubleAndLift<MONAD, MA, NUM>(int num)
            where MONAD : struct, Monad<MA, int>
            where NUM   : struct, Num<int> =>
            Return<MONAD, MA, int>(product<NUM, int>(num, fromInteger<NUM, int>(2)));
    }
}
