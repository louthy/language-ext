using Xunit;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using static LanguageExt.TypeClass;

namespace LanguageExt.Tests
{
    public class TypeClassEQ
    {
        [Fact]
        public void IntEQ()
        {
            Assert.True(IsEqualGeneral<EqInt, int>(10, 10));
            Assert.False(IsEqualGeneral<EqInt, int>(10, 20));
        }

        [Fact]
        public void DoubleEQ()
        {
            Assert.True(IsEqualGeneral<EqDouble, double>(5.0, 5.0));
            Assert.False(IsEqualGeneral<EqDouble, double>(10.0, 20.0));
        }

        [Fact]
        public void StringEQ()
        {
            Assert.True(IsEqualGeneral<EqString, string>("test", "test"));
            Assert.False(IsEqualGeneral<EqString, string>("not", "same"));
        }

        public bool IsEqualGeneral<EQ, A>(A x, A y) where EQ : struct, Eq<A> => 
            equals<EQ, A>(x, y);

    }
}
