using LanguageExt.ClassInstances;
using System;
using Xunit;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;

namespace LanguageExt.Tests
{
    public class TraverseTests
    {
        [Fact]
        public void OptionList()
        {
            var result = traverse<MOption<int>, MLst<int>, Option<int>, Lst<int>, int, int>(
                Some(1),
                x => x % 2 == 0
                    ? List(2, 4, 6, 8, 10)
                    : List(1, 3, 5, 7, 9)
                );

            Assert.True(result == List(1, 3, 5, 7, 9));
        }


        [Fact]
        public void ListList()
        {
            var result = traverse<MLst<int>, MLst<int>, Lst<int>, Lst<int>, int, int>(
                List(1, 2),
                x => x % 2 == 0
                    ? List(2, 4, 6, 8, 10)
                    : List(1, 3, 5, 7, 9)
                );

            Assert.True(result == List(1, 3, 5, 7, 9));
        }
    }
}
