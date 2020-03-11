using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionAsyncT.Sync
{
    public class IdentityTryOptionAsync
    {
        static Task<bool> Eq<A>(TryOptionAsync<Identity<A>> ma, TryOptionAsync<Identity<A>> mb) =>
            EqAsyncClass<TryOptionAsync<Identity<A>>>.EqualsAsync(ma, mb);
        
        [Fact]
        public async void IdentityFailIsFail()
        {
            var ma = Id<TryOptionAsync<int>>(TryOptionAsync<int>(new Exception("fail")));
            var mb = ma.Traverse(Prelude.identity);
            var mc = TryOptionAsync<Identity<int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
        
        [Fact]
        public async void IdentitySuccIsSuccIdentity()
        {
            var ma = Id(TryOptionAsync(1234));
            var mb = ma.Traverse(Prelude.identity);
            var mc = TryOptionAsync(new Identity<int>(1234));

            var mr = await Eq(mb, mc);
            
            Assert.True(mr);
        }
    }
}
