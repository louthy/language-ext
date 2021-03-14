using System;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.QueT.Sync
{
    public class ValidationQue
    {
        [Fact]
        public void FailIsSingletonFail()
        {
            var ma = Fail<Error, Que<int>>(Error.New("alt"));
            var mb = ma.Traverse(identity);
            var mc = Queue(Fail<Error, int>(Error.New("alt")));

            Assert.True(mb == mc);
        }

        [Fact]
        public void SuccessEmptyIsEmpty()
        {
            var ma = Success<Error, Que<int>>(Empty);
            var mb = ma.Traverse(identity);
            var mc = Queue<Validation<Error, int>>();

            Assert.True(mb == mc);
        }

        [Fact]
        public void SuccessNonEmptyQueIsQueSuccesses()
        {
            var ma = Success<Error, Que<int>>(Queue(1, 2, 3, 4));
            var mb = ma.Traverse(identity);
            var mc = Queue(Success<Error, int>(1), Success<Error, int>(2), Success<Error, int>(3), Success<Error, int>(4));

            Assert.True(mb == mc);
        }
    }
}
