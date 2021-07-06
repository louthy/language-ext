using Xunit;
using System.IO;
using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Sys.IO;
using LanguageExt.Sys.Test;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

using Console = LanguageExt.Sys.Console<LanguageExt.Sys.Test.Runtime>;

namespace LanguageExt.Tests
{
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
            var rt = Runtime.New();

            var comp = lines.Sequence(Console.writeLine);
            comp.RunUnit(rt);
            
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
            comp.RunUnit(rt);
            Assert.True(lines == rt.Env.Console.ToSeq(), "sequences don't match");
        }

        static string FailMsg<A>(Fin<A> ma) =>
            ma.Match(Succ: _ => "", Fail: e => e.Message);
    }
}
