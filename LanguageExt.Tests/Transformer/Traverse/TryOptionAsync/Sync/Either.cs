using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionAsyncT.Sync
{
    public class EitherTryOptionAsync
    {
        static Task<bool> Eq<L, R>(TryOptionAsync<Either<L, R>> ma, TryOptionAsync<Either<L, R>> mb) =>
            EqAsyncClass<TryOptionAsync<Either<L, R>>>.EqualsAsync(ma, mb);
        
        [Fact]
        public async void LeftIsSuccLeft()
        {
            var ma = Left<Error, TryOptionAsync<int>>(Error.New("fail"));
            var mb = ma.Sequence();
            var mc = TryOptionAsync(Left<Error, int>(Error.New("fail")));

            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightFailIsFail()
        {
            var ma = Right<Error, TryOptionAsync<int>>(TryOptionAsync<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryOptionAsync<Either<Error, int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }
                
        [Fact]
        public async void RightNoneIsNone()
        {
            var ma = Right<Error, TryOptionAsync<int>>(TryOptionAsync<int>(None));
            var mb = ma.Sequence();
            var mc = TryOptionAsync<Either<Error, int>>(None);

            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightSuccIsSuccRight()
        {
            var ma = Right<Error, TryOptionAsync<int>>(TryOptionAsync(1234));
            var mb = ma.Sequence();
            var mc = TryOptionAsync(Right<Error, int>(1234));

            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }
    }
}
