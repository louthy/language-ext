using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.QueT.Sync
{
    public class OptionUnsafeQue
    {
        [Fact]
        public void NoneIsSingletonNone()
        {
            var ma = OptionUnsafe<Que<int>>.None;
            var mb = ma.Traverse(identity);
            var mc = Queue(OptionUnsafe<int>.None);

            Assert.True(mb == mc);
        }

        [Fact]
        public void SomeEmptyIsEmpty()
        {
            var ma = SomeUnsafe<Que<int>>(Empty);
            var mb = ma.Traverse(identity);
            var mc = Queue<OptionUnsafe<int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void SomeNonEmptyQueIsQueSomes()
        {
            var ma = SomeUnsafe(Queue(1, 2, 3));
            var mb = ma.Traverse(identity);
            var mc = Queue(SomeUnsafe(1), SomeUnsafe(2), SomeUnsafe(3));

            Assert.True(mb == mc);
        }
    }
}
