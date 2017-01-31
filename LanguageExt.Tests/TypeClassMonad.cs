using Xunit;
using System;
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
        public void MonadDoubleIntValueTest()
        {
            Option<int> x = DoubleGenericMonad<MOption<int>, Option<int>>(Some(100));
            Assert.True(x.IfNone(0) == 200);
        }

        [Fact]
        public void MonadDoubleNumValueTest()
        {
            Option<double> x = DoubleGenericMonad<MOption<double>, Option<double>, TDouble, double>(Some(100.0));
            Assert.True(Math.Abs(x.IfNone(0) - 200.0) < 0.000001);
        }

        [Fact]
        public void MonadReturnTest1()
        {
            Option<int> x = DoubleAndLift<MOption<int>, Option<int>>(100);
            Assert.True(x.IfNone(0) == 200);
        }

        [Fact]
        public void MonadReturnTest2()
        {
            Option<int> x = DoubleAndLift<MOption<int>, Option<int>, TInt, int>(100);
            Assert.True(x.IfNone(0) == 200);
        }

        [Fact]
        public void MonadReturnTest3()
        {
            Option<double> x = DoubleAndLift<MOption<double>, Option<double>, TDouble, double>(100.0);
            Assert.True(Math.Abs(x.IfNone(0) -  200.0) < 0.000001);
        }

        public static MA DoubleAndLift<MONAD, MA, NUM, A>(A num)
            where MONAD : struct, Monad<MA, A>
            where NUM   : struct, Num<A> =>
            Return<MONAD, MA, A>(product<NUM, A>(num, fromInteger<NUM, A>(2)));

        public static MA DoubleAndLift<MONAD, MA>(int num)
            where MONAD : struct, Monad<MA, int> =>
            Return<MONAD, MA, int>(product<TInt, int>(num, 2));

        public static MA DoubleGenericMonad<MONAD, MA>(MA ma)
            where MONAD : struct, Monad<MA, int> =>
            bind<MONAD, MONAD, MA, MA, int, int>(ma, a => Return<MONAD, MA, int>(a * 2));

        public static MA DoubleGenericMonad<MONAD, MA, NUM, A>(MA ma)
            where MONAD : struct, Monad<MA, A>
            where NUM   : struct, Num<A> =>
            bind<MONAD, MONAD, MA, MA, A, A>(ma, a => Return<MONAD, MA, A>(product<NUM, A>(a, fromInteger<NUM, A>(2))));
    }
}
