using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances;

public struct FValueTask<A, B> : 
    Functor<ValueTask<A>, ValueTask<B>, A, B>
{
    [Pure]
    public static ValueTask<B> Map(ValueTask<A> ma, Func<A, B> f)
    {
        return Go(ma, f);
        static async ValueTask<B> Go(ValueTask<A> task, Func<A, B> map) =>
            map(await task.ConfigureAwait(false));
    }
}
