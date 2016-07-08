using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using LanguageExt.TypeClass;
using static LanguageExt.TypeClass.Prelude;

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
            eq<EQ, A>(x, y);

    }
}
