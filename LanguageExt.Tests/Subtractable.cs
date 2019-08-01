using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using LanguageExt;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests
{
    public class Subtractable
    {
        [Fact]
        public void OptionalNumericSubtract()
        {
            var x = Some(20);
            var y = Some(10);
            var z = subtract<TInt, int>(x, y);

            Assert.True(z == 10);
        }

        [Fact]
        public void OptionalListSubtract()
        {
            var x = Some(List(1, 2, 3));
            var y = Some(List(1, 2));
            var z = x.SubtractT<TInt, int>(y);

            // 1 - 1 = 0
            // 1 - 2 = -1
            // 2 - 1 = 1
            // 2 - 2 = 0
            // 3 - 1 = 2
            // 3 - 2 = 1

            match(z,
                Some: list =>
                {
                    Assert.True(list.Count == 6);
                    Assert.True(list[0] == 0);
                    Assert.True(list[1] == -1);
                    Assert.True(list[2] == 1);
                    Assert.True(list[3] == 0);
                    Assert.True(list[4] == 2);
                    Assert.True(list[5] == 1);
                },
                None: () => Assert.True(false)
            );
        }

        [Fact]
        public void OptionalSetSubtract()
        {
            var x = Some(Set(1,2,3));
            var y = Some(Set(2,3,4));
            var z = x.SubtractT<TInt, int>(y);
        
            // 1 - 2 = -1
            // 1 - 3 = -2
            // 1 - 4 = -3
            // 2 - 2 =  0
            // 2 - 3 = -1
            // 2 - 4 = -2
            // 3 - 2 = 1
            // 3 - 3 = 0
            // 3 - 4 = -1

            // -3, -2, -1, 0, 1

            match(z,
                Some: set =>
                {
                    Assert.True(set.Count == 5);
                    Assert.True(set.Contains(-3));
                    Assert.True(set.Contains(-2));
                    Assert.True(set.Contains(-1));
                    Assert.True(set.Contains(0));
                    Assert.True(set.Contains(1));
                },
                None: () => Assert.True(false)
            );
        }
    }
}
