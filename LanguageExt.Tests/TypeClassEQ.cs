using Xunit;
using LanguageExt.TypeClasses;
using LanguageExt.Instances;
using static LanguageExt.TypeClass;

namespace LanguageExtTests
{
    public class TypeClassEQ
    {
        [Fact]
        public void IntEQ()
        {
            Assert.True(IsEqualGeneral<EqInt, int>(10, 10));
            Assert.False(IsEqualGeneral<EqInt, int>(10, 20));
        }

        public bool IsEqualGeneral<EQ, A>(A x, A y) where EQ : struct, Eq<A> => 
            equals<EQ, A>(x, y);

    }
}
