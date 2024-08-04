using System.IO;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Sys;
using LanguageExt.Sys.Traits;
using LanguageExt.Traits;
using static LanguageExt.Prelude;
using Newtonsoft.Json;

public class QueueExample<RT>
    where RT : struct,
    Has<Eff<RT>, ConsoleIO>,
    Has<Eff<RT>, DirectoryIO>,
    Has<Eff<RT>, FileIO>
{
    public static Eff<RT, Unit> Issue1065()
    {
        var content   = "test\0test\0test\0"u8.ToArray();
        var memStream = new MemoryStream(100);
        memStream.Write(content, 0, content.Length);
        memStream.Seek(0, SeekOrigin.Begin);

        return repeat(from _51 in SuccessEff(unit)
                      from ln in (from data in liftEff(memStream.ReadByte)
                                  from _ in guard(data != -1, Errors.Cancelled)
                                  select data).FoldUntil(string.Empty, (s, ch) => s + ch, ch => ch == '\0')
                      from _52 in Console<Eff<RT>, RT>.writeLine(ln)
                      select unit)
               | catchM(exception => Console<Eff<RT>, RT>.writeLine(exception.Message));
    }
}

public class Issue1230
{
    public static void Run()
    {
        var finVal = Fin<string>.Succ("hello");
        var json = JsonConvert.SerializeObject(finVal);
    }
}
