using System.IO;
using System.Text;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.Sys;
using LanguageExt.Sys.Traits;
using static LanguageExt.Prelude;

public class QueueExample<RT>
    where RT : struct,
    HasCancel<RT>,
    HasConsole<RT>,
    HasDirectory<RT>,
    HasFile<RT>
{
    public static Aff<RT, Unit> Issue1065()
    {
        var content = Encoding.ASCII.GetBytes("test\0test\0test\0");
        var memStream = new MemoryStream(100);
        memStream.Write(content, 0, content.Length);
        memStream.Seek(0, SeekOrigin.Begin);

        return repeat(
                   from _51 in SuccessEff(unit)
                   from ln in (
                       from data in Eff(memStream.ReadByte)
                       from _ in guard(data != -1, Errors.Cancelled)
                       select data).FoldUntil(string.Empty, (s, ch) => s + (char)ch, ch => ch == '\0')
                   from _52 in Console<RT>.writeLine(ln)
                   select unit)
               | @catch(exception => Console<RT>.writeLine(exception.Message));
    }
}
