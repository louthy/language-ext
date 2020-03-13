using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Sync
{
    public class OptionUnsafeTask
    {
        /*[Fact]
        public async void NoneIsSomeNone()
        {
            var ma = OptionUnsafe<Task<int>>.None;
            var mb = ma.Sequence();
            var mc = SomeAsync(OptionUnsafe<int>.None);

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeNoneIsNone()
        {
            var ma = SomeUnsafe<Task<int>>(None);
            var mb = ma.Sequence();
            var mc = Task<OptionUnsafe<int>>.None;

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeSomeIsSomeSome()
        {
            var ma = SomeUnsafe(SomeAsync(1234));
            var mb = ma.Sequence();
            var mc = SomeAsync(SomeUnsafe(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }*/
    }
}
