using System;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryT.Collections
{
    public class StckTry
    {
        [Fact]
        public void EmptyStackIsSuccEmptyStack()
        {
            Stck<Try<int>> ma = Empty;

            var mb = ma.Traverse(Prelude.identity);

            var mc = TrySucc(Stck<int>.Empty);
            
            Assert.True(default(EqTry<Stck<int>>).Equals(mb, mc));
        }
        
        [Fact]
        public void StackSuccsIsSuccStacks()
        {
            var ma = Stack(TrySucc(1), TrySucc(2), TrySucc(3));

            var mb = ma.Traverse(Prelude.identity);

            var mc = TrySucc(Stack(1, 2, 3));
            
            Assert.True(default(EqTry<Stck<int>>).Equals(mb, mc));

        }
        
        [Fact]
        public void StackSuccAndFailIsFail()
        {
            var ma = Stack(TrySucc(1), TrySucc(2), TryFail<int>(new Exception("fail")));

            var mb = ma.Traverse(Prelude.identity);

            var mc = TryFail<Stck<int>>(new Exception("fail"));

            Assert.True(default(EqTry<Stck<int>>).Equals(mb, mc));
        }
    }
}
