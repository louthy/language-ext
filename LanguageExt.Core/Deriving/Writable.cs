using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    public interface Writable<Supertype, Subtype, W> :
        Writable<Supertype, W>,
        Traits.Natural<Supertype, Subtype>,
        Traits.CoNatural<Supertype, Subtype>
        where Supertype : Writable<Supertype, Subtype, W>, Writable<Supertype, W>
        where Subtype : Writable<Subtype, W>
        where W : Monoid<W>
    {
        /// <summary>
        /// Tell is an action that produces the writer output
        /// </summary>
        /// <param name="item">Item to tell</param>
        /// <typeparam name="W">Writer type</typeparam>
        /// <returns>Structure with the told item</returns>
        static K<Supertype, Unit> Writable<Supertype, W>.Tell(W item) =>
            Supertype.CoTransform(Subtype.Tell(item));

        /// <summary>
        /// Writes an item and returns a value at the same time
        /// </summary>
        static K<Supertype, (A Value, W Output)> Writable<Supertype, W>.Listen<A>(K<Supertype, A> ma) =>
            Supertype.CoTransform(Subtype.Listen(Supertype.Transform(ma)));
        
        /// <summary>
        /// `Pass` is an action that executes the `action`, which returns a value and a
        /// function; it then returns the value with the output having been applied to
        /// the function.
        /// </summary>
        /// <remarks>
        /// For usage, see `Writer.censor` for how it's used to filter the output.
        /// </remarks>
        static K<Supertype, A> Writable<Supertype, W>.Pass<A>(K<Supertype, (A Value, Func<W, W> Function)> action) =>
            Supertype.CoTransform(Subtype.Pass(Supertype.Transform(action)));
    }
}
