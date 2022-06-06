using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Sys;
using LanguageExt.Sys.Traits;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace EffectsExamples
{
    /// <summary>
    /// Retry example
    /// </summary>
    /// <remarks>
    /// Asks you to say hello.
    /// If you don't type 'hello' then an error will be raised and it will retry.
    /// </remarks>
    /// <typeparam name="RT"></typeparam>
    public class RetryExample<RT>
        where RT : struct, 
        HasCancel<RT>, 
        HasConsole<RT>
    {
        static readonly Error Failed = ("I asked you to say hello, and you can't even do that?!");
        
        public static Eff<RT, Unit> main =>
            retry(Schedule.recurs(5),
                  from _ in Console<RT>.writeLine("Say hello")
                  from t in Console<RT>.readLine
                  from e in guard(t == "hello", Failed)  
                  from m in Console<RT>.writeLine("Hi")
                  select unit);
    }
}
