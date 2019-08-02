using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static LanguageExt.Prelude;
using LanguageExt;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests
{
    public class Multiplicable
    {
        [Fact]
        public void OptionalNumericMultiply()
        {
            var x = Some(10);
            var y = Some(20);
            var z = product<TInt, int>(x, y);

            Assert.True(z == 200);
        }
    }
}
