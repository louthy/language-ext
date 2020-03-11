using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryAsyncT.Sync
{
    public class EitherTryAsync
    {
        static Task<bool> Eq<L, R>(TryAsync<Either<L, R>> ma, TryAsync<Either<L, R>> mb) =>
            EqAsyncClass<TryAsync<Either<L, R>>>.EqualsAsync(ma, mb);
        
        [Fact]
        public async void LeftIsSuccLeft()
        {
            var ma = Left<Error, TryAsync<int>>(Error.New("fail"));
            var mb = ma.Sequence();
            var mc = TryAsync(Left<Error, int>(Error.New("fail")));

            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightFailIsFail()
        {
            var ma = Right<Error, TryAsync<int>>(TryAsync<int>(new Exception("fail")));
            var mb = ma.Sequence();
            var mc = TryAsync<Either<Error, int>>(new Exception("fail"));

            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }
        
        [Fact]
        public async void RightSuccIsSuccRight()
        {
            var ma = Right<Error, TryAsync<int>>(TryAsync(1234));
            var mb = ma.Sequence();
            var mc = TryAsync(Right<Error, int>(1234));

            var mr = await Eq(mb, mc);
            Assert.True(mr);
        }
    }
}
