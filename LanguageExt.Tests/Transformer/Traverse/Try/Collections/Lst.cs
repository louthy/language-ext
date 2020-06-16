using System;
using System.Collections.Generic;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryT.Collections
{
    public class LstTry
    {
        [Fact]
        public void EmptyLstIsSuccEmptyLst()
        {
            Lst<Try<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = TrySucc(Lst<int>.Empty);
            
            Assert.True(default(EqTry<Lst<int>>).Equals(mb, mc));
        }
        
        [Fact]
        public void LstSuccsIsSuccLsts()
        {
            var ma = List(TrySucc(1), TrySucc(2), TrySucc(3));

            var mb = ma.Sequence();

            var mc = TrySucc(List(1, 2, 3));
            
            Assert.True(default(EqTry<Lst<int>>).Equals(mb, mc));
        }
        
        [Fact]
        public void LstSuccAndFailIsFail()
        {
            var ma = List(TrySucc(1), TrySucc(2), TryFail<int>(new Exception("fail")));

            var mb = ma.Sequence();

            var mc = TryFail<Lst<int>>(new Exception("fail"));
            
            Assert.True(default(EqTry<Lst<int>>).Equals(mb, mc));
        }
    }
}
