using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Identity.Collections
{
    public class Que
    {
        [Fact]
        public void EmptyQueIsEmpty()
        {
            var ma = Que<Identity<int>>.Empty;

            var mb = ma.Traverse(identity);

            Assert.Equal(Id(Que<int>.Empty), mb);
        }

        [Fact]
        public void QueOfIdentitiesIsIdentityOfQue()
        {
            var ma = Queue(Id(1), Id(3), Id(5));

            var mb = ma.Traverse(identity);

            Assert.Equal(Id(Queue(1, 3, 5)), mb);
        }
    }
}
