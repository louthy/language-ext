using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static LanguageExt.Prelude;
using LanguageExt;

namespace LanguageExtTests
{
    public class Multiplicable
    {
        [Fact]
        public void OptionalNumericMultiply()
        {
            var x = Some(10);
            var y = Some(20);
            var z = x * y;

            Assert.True(z == 200);
        }

        [Fact]
        public void OptionalListMultiply()
        {
            var x = Some(List(2, 4));
            var y = Some(List(1, 2, 3));
            var z = x * y;

            match(z,
                Some: list =>
                {
                    Assert.True(list.Count == 6);
                    Assert.True(list[0] == 2);
                    Assert.True(list[1] == 4);
                    Assert.True(list[2] == 6);
                    Assert.True(list[3] == 4);
                    Assert.True(list[4] == 8);
                    Assert.True(list[5] == 12);
                },
                None: () => Assert.True(false)
            );
        }

        [Fact]
        public void OptionalSetMultiply()
        {
            var x = Some(Set(2, 4));
            var y = Some(Set(1, 2, 3));
            var z = x * y;

            match(z,
                Some: set =>
                {
                    Assert.True(set.Count == 5);
                    Assert.True(set.Contains(2));
                    Assert.True(set.Contains(4));
                    Assert.True(set.Contains(6));
                    Assert.True(set.Contains(8));
                    Assert.True(set.Contains(12));
                },
                None: () => Assert.True(false)
            );
        }
    }
}
