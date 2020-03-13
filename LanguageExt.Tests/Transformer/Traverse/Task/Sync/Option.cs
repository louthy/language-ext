using Xunit;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt.Tests.Transformer.Traverse.TaskT.Sync
{
    public class OptionTask
    {
        /*[Fact]
        public async void NoneIsSomeNone()
        {
            var ma = Option<Task<int>>.None;
            var mb = ma.Sequence();
            var mc = SomeAsync(Option<int>.None);

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeNoneIsNone()
        {
            var ma = Some<Task<int>>(None);
            var mb = ma.Sequence();
            var mc = Task<Option<int>>.None;

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeSomeIsSomeSome()
        {
            var ma = Some(SomeAsync(1234));
            var mb = ma.Sequence();
            var mc = SomeAsync(Some(1234));

            var mr = await (mb == mc);
            
            Assert.True(mr);
        }*/
    }
}
