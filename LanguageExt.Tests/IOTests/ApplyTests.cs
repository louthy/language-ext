using System;
using System.Threading.Tasks;
using Xunit;

namespace LanguageExt.Tests.IOTests;

public class ApplyTests
{
    [Fact]
    public async Task AsyncAsyncSuccessTest()
    {
        var af = Atom(0);
        var aa = Atom(0);
        
        // Create a task that takes longer to return than `fa`.  This relies on `fa` completing first
        // so that aa.Value has been set.  This proves that the applicative parallel behaviour is working 
        var ff = IO.liftAsync(() => Task.Delay(100)
                                        .ToUnit()
                                        .Map(_ => af.Swap(x => x + aa.Value))
                                        .Map(_ => new Func<int, int>(x => x * af.Value)));
        
        var fa = IO.liftAsync(() => Task.Delay(10)
                                        .ToUnit()
                                        .Map(_ => aa.Swap(x => x + 5)));
        
        var fr = ff.Apply(fa);
        var r  = await fr.RunAsync();
        Assert.Equal(25, r);
    }
    
    [Fact]
    public async Task AsyncAsyncFailTest()
    {
        var af = Atom(0);
        var aa = Atom(0);
        
        var ff = IO.liftAsync(() => Task.Delay(10)
                                        .ToUnit()
                                        .Map(_ => af.Swap(x => x + aa.Value))
                                        .Map(_ => new Func<int, int>(x => x * af.Value)));
        
        var fa = IO.liftAsync(() => Task.Delay(100)
                                        .ToUnit()
                                        .Map(_ => aa.Swap(x => x + 5)));
        
        var fr = ff.Apply(fa);
        var r  = await fr.RunAsync();
        Assert.NotEqual(25, r);
    }    
    
    [Fact]
    public async Task AsyncSyncTest()
    {
        var af = Atom(0);
        var aa = Atom(0);
        
        // Create a task that takes longer to return than `fa`.  This relies on `fa` completing first
        // so that aa.Value has been set.  This proves that the applicative parallel behaviour is working 
        var ff = IO.liftAsync(() => Task.Delay(100)
                                        .ToUnit()
                                        .Map(_ => af.Swap(x => x + aa.Value))
                                        .Map(_ => new Func<int, int>(x => x * af.Value)));
        
        var fa = IO.lift(() => aa.Swap(x => x + 5));
        
        var fr = ff.Apply(fa);
        var r  = await fr.RunAsync();
        Assert.Equal(25, r);
    }
    
    [Fact]
    public void SyncSyncTest()
    {
        var ff = IO.pure((int x) => x * 5);
        var fa = IO.pure(5);
        var fr = ff.Apply(fa);
        var r  = fr.Run();
        Assert.Equal(25, r);
    }
    
    [Fact]
    public async Task SyncAsyncTest()
    {
        var ff = IO.liftAsync(() => Task.Delay(100)
                                        .ToUnit()
                                        .Map(_ => new Func<int, int>(x => x * 5)));
        var fa = IO.pure(5);
        var fr = ff.Apply(fa);
        var r  = await fr.RunAsync();
        Assert.Equal(25, r);
    }
}
