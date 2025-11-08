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
        
        IO<Unit> innerLoop(int remaining) =>
            from _1 in state.SwapIO(x => x + 1)
            from _2 in tail(when(remaining > 0, innerLoop(remaining - 1)).As())
            select unit;

        var fullApplication1 =
            from _1 in state.SwapIO(_ => 0)
            from _2 in innerLoop(3)
            from _3 in state.SwapIO(x => x + 1)
            select unit;
        
        var fullApplication2 = 
            from _4 in state.SwapIO(_ => 0)
            from _6 in innerLoop(2).Kind()
            from result in state.SwapIO(x => x * 10)
            select result;
        
        fullApplication1.Run();
        Assert.Equal(5, state.Value);
        
        fullApplication2.Run();
        Assert.Equal(30, state.Value);
    }
    
        
    [Fact]
    public void TailRecursion_WhenUsedImproperly_ShouldThrow()
    {
        IO<Unit> loop(int remaining) =>
            from _ in unitIO
            from _1 in tail(when(remaining > 0, loop(remaining - 1)).As())
            from _2 in unitIO
            select unit;

        Assert.Throws<NotSupportedException>(() => loop(3).Run());
    }
}
