using System;
using LanguageExt.Common;
using Xunit;

namespace LanguageExt.Tests.TraitTests;

public class MonoidKTests
{
    [Fact]
    public static void SomeTest()
    {
        var aio = Atom(10);
        var operation = from v in aio.SwapIO(v => v - 1)
                        from r in v > 0 ? IO.pure(v) : Fail(Error.New("failed"))
                        select r;
        
        var itemsIO = operation.Some();
        var items = itemsIO.Run();
        
        Assert.True(items == [9, 8, 7, 6, 5, 4, 3, 2, 1]);
    }
    
    [Fact]
    public static void ManyTest()
    {
        var aio = Atom(10);
        var operation = from v in aio.SwapIO(v => v - 1)
                        from r in v > 0 ? IO.pure(v) : Fail(Error.New("failed"))
                        select r;
        
        var itemsIO = operation.Many();
        var items   = itemsIO.Run();
        
        Assert.True(items == [9, 8, 7, 6, 5, 4, 3, 2, 1]);
    }
    
    [Fact]
    public static void Some0Test()
    {
        var aio = Atom(1);
        var operation = from v in aio.SwapIO(v => v - 1)
                        from r in v > 0 ? IO.pure(v) : Fail(Error.New("failed"))
                        select r;
        
        var itemsIO = operation.Some();

        try
        {
            var items = itemsIO.Run();
            Assert.Fail("shouldn't get here");
        }
        catch (Exception)
        {
            // ignore
        }
    }
        
    [Fact]
    public static void Some1Test()
    {
        var aio = Atom(2);
        var operation = from v in aio.SwapIO(v => v - 1)
                        from r in v > 0 ? IO.pure(v) : Fail(Error.New("failed"))
                        select r;
        
        var itemsIO = operation.Some();
        var items   = itemsIO.Run();
        
        Assert.True(items == [1]);
    }
        
    [Fact]
    public static void Many0Test()
    {
        var aio = Atom(1);
        var operation = from v in aio.SwapIO(v => v - 1)
                        from r in v > 0 ? IO.pure(v) : Fail(Error.New("failed"))
                        select r;
        
        var itemsIO = operation.Many();
        var items   = itemsIO.Run();
        
        Assert.True(items.IsEmpty);
    }
}
