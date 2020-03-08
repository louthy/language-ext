using System;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryT.Collections
{
    public class ArrTry
    {
        [Fact]
        public void EmptyArrIsSuccEmptyArr()
        {
            Arr<Try<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = TrySucc(Arr<int>.Empty);
            
            Assert.True(default(EqTry<Arr<int>>).Equals(mb, mc));
        }
        
        [Fact]
        public void ArrSuccsIsSuccArrs()
        {
            var ma = Array(Prelude.Try(1), Prelude.Try(2), Prelude.Try(3));

            var mb = ma.Sequence();

            var mc = TrySucc(Array(1, 2, 3));
            
            Assert.True(default(EqTry<Arr<int>>).Equals(mb, mc));
        }
        
        [Fact]
        public void ArrSuccAndFailIsFail()
        {
            var ma = Array(TrySucc(1), TrySucc(2), TryFail<int>(new Exception("fail")));

            var mb = ma.Sequence();

            var mc = TryFail<Arr<int>>(new Exception("fail"));
            
            Assert.True(default(EqTry<Arr<int>>).Equals(mb, mc));
        }
    }
}
