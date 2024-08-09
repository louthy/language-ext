using System;
using Xunit;
using LanguageExt.Sys.Test;

using Console = LanguageExt.Sys.Console<LanguageExt.Eff<LanguageExt.Sys.Test.Runtime>, LanguageExt.Sys.Test.Runtime>;

namespace LanguageExt.Tests;

public class MemoryConsoleTests
{
    [Theory]
    [InlineData("abc")]
    [InlineData("abc\ndef")]
    [InlineData("abc\ndef\nghi")]
    [InlineData("abc\n\n")]
    [InlineData("abc\ndef\n")]
    public void Write_line(string unsplitLines)
    {
        var lines = unsplitLines.Split('\n').AsIterable().ToSeq();
        var rt    = Runtime.New();

        var xs = lines.Traverse(Either<Unit, string>.Right);
        
        var comp = lines.Traverse(Console.writeLine).As();
        comp.Run(rt, EnvIO.New()).ThrowIfFail();

        var clines = rt.Env.Console.AsIterable().ToSeq();
        Assert.True(lines == clines, $"sequences don't match {lines} != {clines}");
    }
        
    [Theory]
    [InlineData("abc")]
    [InlineData("abc\ndef")]
    [InlineData("abc\ndef\nghi")]
    [InlineData("abc\n\n")]
    [InlineData("abc\ndef\n")]
    public void Read_line_followed_by_write_line(string unsplitLines)
    {
        // Prep the runtime and the keyboard buffer with the typed lines
        var rt    = Runtime.New();
        var lines = unsplitLines.Split('\n').AsIterable().ToSeq();
        lines.Iter(line => rt.Env.Console.WriteKeyLine(line));

        // test
        var comp = repeatIO(from l in Console.readLine
                            from _ in Console.writeLine(l)
                            select unit).As() | unitEff;
            
        // run and assert
        comp.Run(rt, EnvIO.New()).ThrowIfFail();
        Assert.True(lines == rt.Env.Console.AsIterable().ToSeq(), "sequences don't match");
    }
}
