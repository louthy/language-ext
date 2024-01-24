using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances;

public struct FTask<A, B> : 
    Functor<Task<A>, Task<B>, A, B>
{
    [Pure]
    public static Task<B> Map(Task<A> ma, Func<A, B> f)
    {
        return Go(ma, f);
        static async Task<B> Go(Task<A> task, Func<A, B> map) =>
            map(await task.ConfigureAwait(false));
    }
}
