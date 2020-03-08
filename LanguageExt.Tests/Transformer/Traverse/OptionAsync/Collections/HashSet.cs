using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionAsyncT.Collections
{
    public class HashSetOptionAsync
    {
        // TODO We'd need a HashSetAsync to hold Async types
        //      Because the call to GetHashCode is synchronous.  So, a type that uses `GetHashCodeAsync` would all
        //      this to work.  Going to leave this type here, so it doesn't get implemented again by mistake. 
        
        // [Fact]
        // public async void EmptyIsSomeEmpty()
        // {
        //     var ma = HashSet<OptionAsync<int>>();
        //
        //     var mb = ma.Sequence();
        //
        //     var mc = SomeAsync(HashSet<int>());
        //     
        //     Assert.True(await (mb == mc));        
        // }
        //
        // [Fact]
        // public async void CollectionOfSomesIsSomeCollection()
        // {
        //     var ma = HashSetAsync(SomeAsync(1), SomeAsync(2), SomeAsync(3));
        //
        //     var mb = ma.Sequence();
        //
        //     var mc = SomeAsync(HashSetAsync(1, 2, 3));
        //
        //     Assert.True(await (mb == mc));
        // }
        //
        // [Fact]
        // public async void CollectionOfSomesAndNonesIsNone()
        // {
        //     var ma = HashSetAsync(SomeAsync(1), SomeAsync(2), None);
        //
        //     var mb = ma.Sequence();
        //
        //     Assert.True(await (mb == None));
        // }
    }
}
