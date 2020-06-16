using System;
using LanguageExt.ClassInstances;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionT.Collections
{
    public class StckTryOption
    {
        [Fact]
        public void EmptyStackIsSuccEmptyStack()
        {
            Stck<TryOption<int>> ma = Empty;

            var mb = ma.Traverse(identity);

            var mc = TryOptionSucc(Stck<int>.Empty);

            Assert.True(default(EqTryOption<Stck<int>>).Equals(mb, mc));
        }

        [Fact]
        public void StackSuccsIsSuccStacks()
        {
            var ma = Stack(TryOptionSucc(1), TryOptionSucc(2), TryOptionSucc(3));

            var mb = ma.Traverse(identity);

            var mc = TryOptionSucc(Stack(1, 2, 3));

            Assert.True(default(EqTryOption<Stck<int>>).Equals(mb, mc));
        }

        [Fact]
        public void StackSuccAndFailIsFail()
        {
            var ma = Stack(TryOptionSucc(1), TryOptionSucc(2), TryOptionFail<int>(new Exception("fail")));

            var mb = ma.Traverse(identity);

            var mc = TryOptionFail<Stck<int>>(new Exception("fail"));

            Assert.True(default(EqTryOption<Stck<int>>).Equals(mb, mc));
        }
    }
}
