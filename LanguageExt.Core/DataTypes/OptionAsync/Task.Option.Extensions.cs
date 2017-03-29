using LanguageExt.ClassInstances;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static class TaskOptionAsyncExtensions
    {
        public static OptionAsync<A> ToAsync<A>(this Task<Option<A>> ma) =>
            new OptionAsync<A>(OptionDataAsync.Lazy(async () =>
            {
                var a = await ma;
                return a.IsSome
                    ? new Result<A>(a.Value)
                    : Result<A>.None;
            }));
    }
}
