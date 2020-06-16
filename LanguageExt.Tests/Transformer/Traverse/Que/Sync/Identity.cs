using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.QueT.Sync
{
    public class IdentityQue
    {
        [Fact]
        public void IdEmptyIsEmpty()
        {
            var ma = Id<Que<int>>(Empty);
            var mb = ma.Traverse(identity);
            var mc = Queue<Identity<int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void IdNonEmptyQueIsQueId()
        {
            var ma = Id(Queue(1, 2, 3));
            var mb = ma.Traverse(identity);
            var mc = Queue(Id(1), Id(2), Id(3));

            Assert.True(mb == mc);
        }
    }
}
