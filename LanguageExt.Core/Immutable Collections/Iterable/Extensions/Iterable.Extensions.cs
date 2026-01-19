#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System.Text;
using LanguageExt.Traits;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class IterableExtensions
{
    extension<A>(Iterable<Iterable<A>> ma)
    {
        public Iterable<A> Flatten() =>
            ma.Bind(identity);
    }

    /// <param name="items">sequence</param>
    /// <typeparam name="A">sequence item type</typeparam>
    extension<A>(K<Iterable, A> items)
    {
        public Iterable<A> As() =>
            (Iterable<A>)items;
        
        public IterableIO<A> AsIterableIO() =>
            new(IteratorIO.lift(items.As().iterator));
    }
    
    /// <param name="items">sequence</param>
    /// <typeparam name="A">sequence item type</typeparam>
    extension(K<Iterable, string> items)
    {
        /// <summary>
        /// Concatenate all strings into one
        /// </summary>
        [Pure]
        public string Concat()
        {
            var sb = new StringBuilder();
            foreach (var x in +items)
            {
                sb.Append(x);
            }
            return sb.ToString();
        }
    }
}
