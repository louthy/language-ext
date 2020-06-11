using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.QueT.Sync
{
    public class OptionQue
    {
        [Fact]
        public void NoneIsSingletonNone()
        {
            var ma = Option<Que<int>>.None;
            var mb = ma.Traverse(identity);
            var mc = Queue(Option<int>.None);

            Assert.True(mb == mc);
        }

        [Fact]
        public void SomeEmptyIsEmpty()
        {
            var ma = Some<Que<int>>(Empty);
            var mb = ma.Traverse(identity);
            var mc = Queue<Option<int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void SomeNonEmptyQueIsQueSomes()
        {
            var ma = Some(Queue(1, 2, 3));
            var mb = ma.Traverse(identity);
            var mc = Queue(Some(1), Some(2), Some(3));

            Assert.True(mb == mc);
        }
    }
}
