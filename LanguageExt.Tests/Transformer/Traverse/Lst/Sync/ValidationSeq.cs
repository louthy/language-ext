using System;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Lst.Sync
{
    public class ValidationSeqLst
    {
        [Fact]
        public void FailIsSingletonFail()
        {
            var ma = Fail<Error, Lst<int>>(Error.New("alt"));
            var mb = ma.Sequence();
            var mc = List(Fail<Error, int>(new Exception("alt")));

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SuccessEmptyIsEmpty()
        {
            var ma = Success<Error, Lst<int>>(Empty);
            var mb = ma.Sequence();
            var mc = List<Validation<Error, int>>();

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SuccessNonEmptyLstIsLstSuccesses()
        {
            var ma = Success<Error, Lst<int>>(List(1, 2, 3, 4));
            var mb = ma.Sequence();
            var mc = List(Success<Error, int>(1), Success<Error, int>(2), Success<Error, int>(3), Success<Error, int>(4));
            
            Assert.True(mb == mc);
        }
    }
}
