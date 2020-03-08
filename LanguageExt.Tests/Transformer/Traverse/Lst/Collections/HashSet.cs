using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.Lst.Collections
{
    public class HashSetLst
    {
        [Fact]
        public void EmptyEmptyIsEmptyEmpty()
        {
            HashSet<Lst<int>> ma = Empty;

            var mb = ma.Sequence();

            var mc = Lst<HashSet<int>>.Empty;
            
            Assert.True(mb == mc);
        }

        [Fact]
        public void HashSetLstCrossProduct()
        {
            var ma = HashSet(List(1, 2), List(10, 20, 30));

            var mb = ma.Sequence();

            // TODO: HashSet ordering is undefined ...
            //       On .NET Core this test produces a consistent result and results in the `mc` structure below.
            //       On .NET Framework it in non-deterministic.  This implies that the hash-code generation is non-
            //       deterministic, and that should be investigated, because the only type we don't control in
            //       this test is `int` and it usually returns itself from GetHashCode, so all invocations of this test
            //       should produce the exact same results, except it doesn't on .NET Framework, and sometimes passes
            //       and other times doesn't.
            //
            //       The ordering below is to side-step that until it can be investigated fully.  The code technically
            //       works, because nothing should rely on the order of values in a HashSet, so the test deserves to pass.
            mb = mb.OrderBy(x => x.ToArray()[1])
                   .OrderBy(x => x.ToArray()[0])
                   .Freeze();
            
            var mc = List(HashSet(1, 10), HashSet(1, 20), HashSet(1, 30), HashSet(2, 10), HashSet(2, 20), HashSet(2, 30));

            var tb = mb.ToString();
            var tc = mc.ToString();
            
            Assert.True(mb == mc);
        }
        
                
        [Fact]
        public void HashSetOfEmptiesAndNonEmptiesIsEmpty()
        {
            var ma = HashSet(List<int>(), List(1, 2, 3));

            var mb = ma.Sequence();

            var mc = Lst<HashSet<int>>.Empty;
            
            Assert.True(mb == mc);
        }
        
        [Fact]
        public void HashSetOfEmptiesIsEmpty()
        {
            var ma = HashSet(List<int>(), List<int>());

            var mb = ma.Sequence();

            var mc = Lst<HashSet<int>>.Empty;
            
            Assert.True(mb == mc);
        }
    }
}
