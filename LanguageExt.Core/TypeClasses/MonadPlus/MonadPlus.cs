using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.TypeClasses
{
    [Typeclass]
    public interface MonadPlus<A> : Monad<A>
    {
        /// <summary>
        /// Associative binary operation
        /// </summary>
        MonadPlus<A> Plus(MonadPlus<A> a, MonadPlus<A> b);

        /// <summary>
        /// Neutral element (None in Option for example)
        /// </summary>
        MonadPlus<A> Zero();
    }
}
