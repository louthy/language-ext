using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Sys;
using LanguageExt.Sys.Traits;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace EffectsExamples;

/// <summary>
/// Retry example
/// </summary>
/// <remarks>
/// Asks you to say hello.
/// If you don't type 'hello' then an error will be raised and it will retry.
/// </remarks>
/// <typeparam name="RT"></typeparam>
public class RetryExample<RT> 
    where RT : 
        Has<Eff<RT>, ConsoleIO>
{
    static readonly Error Failed = (Error)"I asked you to say hello, and you can't even do that?!";
        
    public static Eff<RT, Unit> main =>
        retryIO(Schedule.recurs(5),
                from _ in Console<Eff<RT>, RT>.writeLine("Say hello")
                from t in Console<Eff<RT>, RT>.readLine
                from e in guard(t == "hello", Failed)  
                from m in Console<Eff<RT>, RT>.writeLine("Hi")
                select unit).As();
}
