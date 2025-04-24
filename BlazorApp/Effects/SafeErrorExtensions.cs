using LanguageExt.Common;
using LanguageExt.Traits;

namespace BlazorApp.Effects;

public static class SafeErrorExtensions
{
    /// <summary>
    /// Catch exceptional errors and make them safe errors by wrapping up the exception
    /// in an `Inner` property and returning with 'There was an error' as the message. 
    /// </summary>
    /// <param name="ma"></param>
    /// <typeparam name="M"></typeparam>
    /// <typeparam name="A"></typeparam>
    /// <returns></returns>
    public static K<M, A> SafeError<M, A>(this K<M, A> ma)
        where M : Fallible<M> =>
        ma.Catch(e => e.IsExceptional, e => Error.New("There was an error", e));
}
