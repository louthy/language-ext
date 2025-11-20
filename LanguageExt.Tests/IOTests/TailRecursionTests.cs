using System;
using Xunit;

namespace LanguageExt.Tests.IOTests;

public class TailRecursionTests
{
    private record TestIO<A>(IO<A> io) : K<TestIO, A>
    {
        public A Run() => io.Run();
    };
    
    private class TestIO : Deriving.MonadIO<TestIO, IO>
    {
        public static K<IO, A> Transform<A>(K<TestIO, A> fa) => ((TestIO<A>)fa).io;

        public static K<TestIO, A> CoTransform<A>(K<IO, A> fa) => new TestIO<A>(fa.As());
    }
    
    private readonly Atom<int> State = Atom(0);

    [Fact]
    public void TailRecursionInApplication_WhenPlainIO_ShouldNotThrow()
    {
        IO<Unit> innerLoop(int remaining) =>
            from _1 in increment()
            from _2 in tail(when(remaining > 0, innerLoop(remaining - 1)).As())
            select unit;

        var app =
            from _1 in reset()
            from _2 in innerLoop(3)
            from result in multiply()
            select result;
        
        var actual = app.Run();
        Assert.Equal(40, actual);
    }

    private IO<int> reset() => State.SwapIO(_ => 0);

    private IO<int> increment() => State.SwapIO(x => x + 1);
    
    private IO<int> multiply() => State.SwapIO(x => x * 10);


    [Fact]
    public void TailRecursionInApplication_WhenIOHKT_ShouldNotThrow()
    {
        IO<Unit> innerLoop(int remaining) =>
            from _1 in increment()
            from _2 in tail(when(remaining > 0, innerLoop(remaining - 1)).As()).Kind()
            select unit;

        var app =
            from _1 in reset()
            from _2 in innerLoop(3)
            from result in multiply()
            select result;
        
        var actual = app.Run();
        Assert.Equal(40, actual);
    }
    
    [Fact]
    public void TailRecursionInApplication_WhenLiftIO_ShouldNotThrow()
    {
        TestIO<Unit> innerLoop(int remaining) => (TestIO<Unit>)
            from _1 in new TestIO<int>(increment())
            from _2 in tail(when(remaining > 0, innerLoop(remaining - 1).io).As())
            select unit;

        var app = (TestIO<int>)
            from _1 in reset()
            from _2 in innerLoop(3)
            from result in multiply()
            select result;
        
        var actual = app.Run();
        Assert.Equal(40, actual);
    }
        
    [Fact]
    public void TailRecursionUsedImproperly_WhenPlainIO_ShouldThrow()
    {
        IO<Unit> loop(int remaining) =>
            from _ in unitIO
            from _1 in tail(when(remaining > 0, loop(remaining - 1)).As())
            from _2 in unitIO
            select unit;

        Assert.Throws<NotSupportedException>(() => loop(3).Run());
    }
    
    [Fact]
    public void TailRecursionUsedImproperly_WhenIOHKT_ShouldThrow()
    {
        IO<Unit> loop(int remaining) =>
            from _ in unitIO
            from _1 in tail(when(remaining > 0, loop(remaining - 1)).As()).Kind()
            from _2 in unitIO
            select unit;

        Assert.Throws<NotSupportedException>(() => loop(3).Run());
    }
    
        
    [Fact]
    public void TailRecursionUsedImproperly_WhenLiftIO_ShouldThrow()
    {
        TestIO<Unit> loop(int remaining) => (TestIO<Unit>)
            from _ in new TestIO<Unit>(unitIO)
            from _1 in tail(when(remaining > 0, loop(remaining - 1).io).As())
            from _2 in unitIO
            select unit;

        Assert.Throws<NotSupportedException>(() => loop(3).Run());
    }
}
