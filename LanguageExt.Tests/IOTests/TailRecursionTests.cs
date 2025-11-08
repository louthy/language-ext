using System;
using Xunit;

namespace LanguageExt.Tests.IOTests;

public class TailRecursionTests
{
    private record TestIO<A>(IO<A> io) : K<TestIO, A>;
    
    private class TestIO : Deriving.MonadIO<TestIO, IO>
    {
        public static K<IO, A> Transform<A>(K<TestIO, A> fa) => ((TestIO<A>)fa).io;

        public static K<TestIO, A> CoTransform<A>(K<IO, A> fa) => new TestIO<A>(fa.As());
    }
    
    [Fact]
    public void TailRecursion_WhenUsedInMiddleOfApplication_ShouldNotThrow()
    {
        var state = Atom(0);

        IO<Unit> loopHelper(Func<int, IO<Unit>> recur, int remaining) =>
            remaining > 0 ? recur(remaining - 1) : unitIO;
        
        IO<Unit> innerLoop1(int remaining) =>
            from _1 in state.SwapIO(x => x + 1)
            from _2 in tail(loopHelper(innerLoop1, remaining))
            select unit;
        
        IO<Unit> innerLoop2(int remaining) =>
            from _1 in state.SwapIO(x => x + 1)
            from _2 in tail(loopHelper(innerLoop2, remaining)).Kind()
            select unit;
        
        TestIO<Unit> innerLoop3(int remaining) => (TestIO<Unit>)
            from _1 in new TestIO<Unit>(unitIO)
            from _2 in tail(loopHelper(x => innerLoop3(x).io, remaining))
            select unit;

        var fullApplication = (TestIO<Unit>)
            from _1 in state.SwapIO(_ => 0)
            from _2 in innerLoop1(2)
            from _3 in innerLoop2(2)
            from _4 in innerLoop3(2)
            from _5 in state.SwapIO(x => x + 1)
            select unit;
        
        fullApplication.io.Run();
        Assert.Equal(10, state.Value);
    }
    
        
    [Fact]
    public void TailRecursion_WhenUsedImproperly_ShouldThrow()
    {
        IO<Unit> loop1(int remaining) =>
            from _ in unitIO
            from _1 in tail(when(remaining > 0, loop1(remaining - 1)).As())
            from _2 in unitIO
            select unit;
        
        IO<Unit> loop2(int remaining) =>
            from _ in unitIO
            from _1 in tail(when(remaining > 0, loop2(remaining - 1)).As()).Kind()
            from _2 in unitIO
            select unit;

        Assert.Throws<NotSupportedException>(() => loop1(3).Run());
        Assert.Throws<NotSupportedException>(() => loop2(3).Run());
    }
}
