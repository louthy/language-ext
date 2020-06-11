using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryT.Collections
{
    public class IEnumerableTry
    {
        [Fact]
        public void EmptyIEnumerableIsSuccEmptyIEnumerable()
        {
            var ma = Enumerable.Empty<Try<int>>();

            var mb = ma.Sequence();

            var mr = mb.Map(b => ma.Count() == b.Count())
                       .IfFail(false);
            
            Assert.True(mr);
        }

        [Fact]
        public void IEnumerableSuccIsSuccIEnumerables()
        {
            var ma = new[] {TrySucc(1), TrySucc(2), TrySucc(3)}.AsEnumerable();

            var mb = ma.Sequence();

            Assert.True(mb.Map(b => default(EqEnumerable<int>).Equals(b, new[] {1, 2, 3}.AsEnumerable())).IfFail(false));
        }

        [Fact]
        public void IEnumerableSuccAndFailIsFail()
        {
            var ma = new[] {TrySucc(1), TrySucc(2), TryFail<int>(new Exception("fail"))}.AsEnumerable();

            var mb = ma.Sequence();

            var mc = TryFail<IEnumerable<int>>(new Exception("fail"));
            
            Assert.True(default(EqTry<IEnumerable<int>>).Equals(mb, mc));

        }
    }
}
