using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryAsyncT.Async
{
    public class OptionAsyncTryAsync
    {
        static Task<bool> Eq<A>(TryAsync<OptionAsync<A>> ma, TryAsync<OptionAsync<A>> mb) =>
            EqAsyncClass<TryAsync<OptionAsync<A>>>.EqualsAsync(ma, mb);
 
        [Fact]
        public async void NoneIsSuccNone()
        {
            var ma = OptionAsync<TryAsync<int>>.None;
            var mb = ma.Sequence();
            var mc = TryAsync(OptionAsync<int>.None);

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeFailIsFail()
        {
            var ma = SomeAsync(TryAsync<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryAsync<OptionAsync<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void SomeSuccIsSuccSome()
        {
            var ma = SomeAsync(TryAsync(1234));
            var mb = ma.Sequence();
            var mc = TryAsync(SomeAsync(1234));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
