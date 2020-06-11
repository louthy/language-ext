using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryAsyncT.Sync
{
    public class OptionTryAsync
    {
        static Task<bool> Eq<A>(TryAsync<Option<A>> ma, TryAsync<Option<A>> mb) =>
            EqAsyncClass<TryAsync<Option<A>>>.EqualsAsync(ma, mb);
        
        [Fact]
        public async void NoneIsSuccNone()
        {
            var ma = Option<TryAsync<int>>.None;
            var mb = ma.Sequence();
            var mc = TryAsync(Option<int>.None);

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeFailIsFail()
        {
            var ma = Some<TryAsync<int>>(TryAsync<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryAsync<Option<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeSuccIsSuccSome()
        {
            var ma = Some(TryAsync(1234));
            var mb = ma.Sequence();
            var mc = TryAsync(Some(1234));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
