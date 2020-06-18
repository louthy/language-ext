using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Stck.Sync
{
    public class Try
    {
        [Fact]
        public void SuccEmptyIsEmpty()
        {
            var ma = TrySucc<Stck<int>>(Empty);
            var mb = ma.Traverse(identity);
            var mc = Stack<Try<int>>();

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void SuccNonEmptyIsStackSuccess()
        {
            var ma = TrySucc(Stack(1, 2, 3, 4));
            var mb = ma.Traverse(identity);
            var mc = Stack(TrySucc(4), TrySucc(3), TrySucc(2), TrySucc(1));
        }

        [Fact]
        public void FailIsStackFail()
        {
            var ma = TryFail<Stck<int>>(new System.Exception("Fail"));
            var mb = ma.Traverse(identity);
            var mc = Stack(TryFail<int>(new System.Exception("Fail")));

            Assert.Equal(mc, mb);
        }
    }
}
