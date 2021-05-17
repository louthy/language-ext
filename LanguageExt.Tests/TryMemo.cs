using System;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class TryMemo
    {
        [Fact]
        public void WhenNoExcpetion_MemoizeValue()
        {
            var counter = 0;
            var sut = Try(() => counter++).Memo();
            sut();
            sut();
            sut();
            Assert.Equal(1, counter);
        }

        [Fact]
        public void WhenExcpetion_DontMemoizeValue()
        {
            var counter = 0;
            var sut = Try(() => counter++ == 0 ? throw new Exception() : counter).Memo();
            sut();
            sut();
            sut();
            Assert.Equal(2, counter);
        }
    }
}
