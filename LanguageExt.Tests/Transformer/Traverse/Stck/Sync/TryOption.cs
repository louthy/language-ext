using Xunit;

namespace LanguageExt.Tests.Transformer.Traverse.Stck.Sync
{
    public class TryOption
    {
        [Fact]
        public void SuccEmptyIsEmpty()
        {
            var ma = TryOptionSucc<Stck<int>>(Empty);
            var mb = ma.Traverse(identity);
            var mc = Stack<TryOption<int>>();

            Assert.Equal(mc, mb);
        }

        [Fact]
        public void SuccNonEmptyIsStackSuccess()
        {
            var ma = TryOptionSucc(Stack(1, 2, 3, 4));
            var mb = ma.Traverse(identity);
            var mc = Stack(TryOptionSucc(1), TryOptionSucc(2), TryOptionSucc(3), TryOptionSucc(4));

            var eq = mb == mc;
            Assert.True(eq);
        }

        [Fact]
        public void FailStackIsStackFail()
        {
            var ma = TryOptionFail<Stck<int>>(new System.Exception("Fail"));
            var mb = ma.Traverse(identity);
            var mc = Stack(TryOptionFail<int>(new System.Exception("Fail")));

            var eq = mb == mc;
            Assert.True(eq);
        }
    }
}
