#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System.Text;
using LanguageExt.Traits;
using System.Threading.Tasks;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class IterableIOExtensions
{
    extension<A>(IterableIO<IterableIO<A>> ma)
    {
        public IterableIO<A> Flatten() =>
            ma.Bind(identity);
    }

    /// <param name="list">sequence</param>
    /// <typeparam name="A">sequence item type</typeparam>
    extension<A>(K<IterableIO, A> list)
    {
        public IterableIO<A> As() =>
            (IterableIO<A>)list;
    }
    
    /// <param name="items">sequence</param>
    /// <typeparam name="A">sequence item type</typeparam>
    extension(K<IterableIO, string> items)
    {
        /// <summary>
        /// Concatenate all strings into one
        /// </summary>
        [Pure]
        public IO<string> Concat()
        {
            return IO.liftVAsync(go);
            async ValueTask<string> go(EnvIO env)
            {
                var sb = new StringBuilder();
                await foreach (var x in await items.As().AsAsyncEnumerable().RunAsync(env))
                {
                    sb.Append(x);
                }
                return sb.ToString();
            }
        }
    }    
}
