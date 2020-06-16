using System;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryAsyncT.Sync
{
    public class ValidationSeqTryAsync
    {
        static Task<bool> Eq<L, R>(TryAsync<Validation<L, R>> ma, TryAsync<Validation<L, R>> mb) =>
            EqAsyncClass<TryAsync<Validation<L, R>>>.EqualsAsync(ma, mb);
        
        [Fact]
        public async void FailIsSuccLeft()
        {
            var ma = Fail<Error, TryAsync<int>>(Error.New("fail"));
            var mb = ma.Sequence();
            var mc = TryAsync(Fail<Error, int>(Error.New("fail")));

            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }
        
        [Fact]
        public async void SuccessFailIsFail()
        {
            var ma = Success<Error, TryAsync<int>>(TryAsync<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryAsync<Validation<Error, int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightSuccIsSuccRight()
        {
            var ma = Success<Error, TryAsync<int>>(TryAsync(1234));
            var mb = ma.Sequence();
            var mc = TryAsync(Success<Error, int>(1234));

            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }
    }
}
