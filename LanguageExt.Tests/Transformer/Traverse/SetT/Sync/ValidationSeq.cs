using System;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SetT.Sync
{
    public class ValidationSeqSet
    {
        // TODO: OrdDefault
        // [Fact]
        // public void FailIsSingletonFail()
        // {
        //     var ma = Fail<Error, Set<int>>(Error.New("alt"));
        //     var mb = ma.Sequence();
        //     var mc = Set(Fail<Error, int>(new Exception("alt")));
        //
        //     Assert.True(mb == mc);
        // }
        
        [Fact]
        public void SuccessEmptyIsEmpty()
        {
            var ma = Success<Error, Set<int>>(Empty);
            var mb = ma.Sequence();
            var mc = Set<Validation<Error, int>>();

            Assert.True(mb == mc);
        }
        
        [Fact]
        public void SuccessNonEmptySetIsSetSuccesses()
        {
            var ma = Success<Error, Set<int>>(Set(1, 2, 3, 4));
            var mb = ma.Sequence();
            var mc = Set(Success<Error, int>(1), Success<Error, int>(2), Success<Error, int>(3), Success<Error, int>(4));
            
            Assert.True(mb == mc);
        }
    }
}
