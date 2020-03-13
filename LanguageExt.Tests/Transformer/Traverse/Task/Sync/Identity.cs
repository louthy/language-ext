using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Sync
{
    public class IdentityTask
    {
        /*[Fact]
        public async void IdentityNoneIsNone()
        {
            var ma = Id<Task<int>>(None);
            var mb = ma.Traverse(Prelude.identity);
            var mc = Task<Identity<int>>.None;

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void IdentitySomeIsSomeIdentity()
        {
            var ma = Id<Task<int>>(1234);
            var mb = ma.Traverse(Prelude.identity);
            var mc = SomeAsync(new Identity<int>(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }*/
    }
}
