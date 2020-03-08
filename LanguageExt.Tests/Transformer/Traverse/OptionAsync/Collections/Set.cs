using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.OptionAsyncT.Collections
{
    public class SetOptionAsync
    {
        // TODO We need SetAsync to hold Async types or a way of constructing a Set from Async types
        //      Because the call to GetHashCode is synchronous.  So, a type that uses `GetHashCodeAsync` would all
        //      this to work.  Going to leave this type here, so it doesn't get implemented again by mistake. 
        //
        // [Fact]
        // public async void EmptyIsSomeEmpty()
        // {
        //     var ma = Set<OptionAsync<int>>();
        //
        //     var mb = ma.Sequence();
        //
        //     var mc = SomeAsync(Set<int>());
        //     
        //     Assert.True(await (mb == mc));        
        // }
        //
        // [Fact]
        // public async void CollectionOfSomesIsSomeCollection()
        // {
        //     var ma = Set(SomeAsync(1), SomeAsync(2), SomeAsync(3));
        //
        //     var mb = ma.Sequence();
        //
        //     var mc = SomeAsync(Set(1, 2, 3));
        //
        //     Assert.True(await (mb == mc));
        // }
        //
        // [Fact]
        // public async void CollectionOfSomesAndNonesIsNone()
        // {
        //     var ma = Set(SomeAsync(1), SomeAsync(2), None);
        //
        //     var mb = ma.Sequence();
        //
        //     Assert.True(await (mb == None));
        // }
    }
}
