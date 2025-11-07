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
            from _2 in when(remaining > 0, tail(innerLoop(remaining - 1)))
            select unit;

        var fullApplication = (TestIO<int>)
            from _1 in state.SwapIO(_ => 0)
            from _2 in innerLoop(3)
            from _3 in state.SwapIO(x => x + 1)
            from _4 in innerLoop(3).Kind()
            from _5 in new TestIO<Unit>(innerLoop(2))
            from result in state.SwapIO(x => x * 10)
            select result;

        fullApplication.io.Run();
        Assert.Equal(120, state.Value);
    }
    
        
    [Fact]
    public void TailRecursion_WhenUsedImproperly_ShouldThrow()
    {
        IO<Unit> loop(int remaining) => (IO<Unit>)
            from _ in unitIO
            from _1 in when(remaining > 0, tail(loop(remaining - 1)))
            from _2 in unitIO
            select unit;

        Assert.Throws<NotSupportedException>(() => loop(3).Run());
    }
}
