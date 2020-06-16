using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryAsyncT.Sync
{
    public class IdentityTryAsync
    {
        static Task<bool> Eq<A>(TryAsync<Identity<A>> ma, TryAsync<Identity<A>> mb) =>
            EqAsyncClass<TryAsync<Identity<A>>>.EqualsAsync(ma, mb);
        
        [Fact]
        public async void IdentityFailIsFail()
        {
            var ma = Id<TryAsync<int>>(TryAsync<int>(new Exception("fail")));
            var mb = ma.Traverse(Prelude.identity);
            var mc = TryAsync<Identity<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void IdentitySuccIsSuccIdentity()
        {
            var ma = Id(TryAsync(1234));
            var mb = ma.Traverse(Prelude.identity);
            var mc = TryAsync(new Identity<int>(1234));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
