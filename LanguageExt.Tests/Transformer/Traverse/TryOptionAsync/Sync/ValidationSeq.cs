using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionAsyncT.Sync
{
    public class ValidationSeqTryOptionAsync
    {
        static Task<bool> Eq<L, R>(TryOptionAsync<Validation<L, R>> ma, TryOptionAsync<Validation<L, R>> mb) =>
            EqAsyncClass<TryOptionAsync<Validation<L, R>>>.EqualsAsync(ma, mb);
        
        [Fact]
        public async void FailIsSuccLeft()
        {
            var ma = Fail<Error, TryOptionAsync<int>>(Error.New("fail"));
            var mb = ma.Sequence();
            var mc = TryOptionAsync(Fail<Error, int>(Error.New("fail")));

            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccessFailIsFail()
        {
            var ma = Success<Error, TryOptionAsync<int>>(TryOptionAsync<int>(None));
            var mb = ma.Sequence();
            var mc = TryOptionAsync<Validation<Error, int>>(None);

            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightSuccIsSuccRight()
        {
            var ma = Success<Error, TryOptionAsync<int>>(TryOptionAsync(1234));
            var mb = ma.Sequence();
            var mc = TryOptionAsync(Success<Error, int>(1234));

            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }
    }
}
