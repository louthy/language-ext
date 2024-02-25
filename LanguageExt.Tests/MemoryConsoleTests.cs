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
        var lines = unsplitLines.Split('\n').ToSeq();
        var rt    = Runtime.New();

        var comp = lines.Traverse(Console.writeLine).As();
        ignore(comp.Run(rt, EnvIO.New()));
            
        Assert.True(lines == rt.Env.Console.ToSeq(), "sequences don't match");
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
        var lines = unsplitLines.Split('\n').ToSeq();
        lines.Iter(line => rt.Env.Console.WriteKeyLine(line));

        // test
        var comp = repeat(from l in Console.readLine
                          from _ in Console.writeLine(l)
                          select unit) | unitEff;
            
        // run and assert
        ignore(comp.Run(rt, EnvIO.New()));
        Assert.True(lines == rt.Env.Console.ToSeq(), "sequences don't match");
    }
}
