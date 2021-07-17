using System;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Used by various error producing monads to have a contextual `where`
    /// </summary>
    /// <remarks>
    /// See `Prelude.guard(...)`
    /// </remarks>
    public readonly struct Guard<E>
    {
        public readonly bool Flag;
        readonly Func<E> onFalse;

        public Guard(bool flag, Func<E> onFalse) =>
            (Flag, this.onFalse) = (flag, onFalse ?? throw new ArgumentNullException(nameof(onFalse)));

        public Guard(bool flag, E onFalse)
        {
            if(isnull(onFalse)) throw new ArgumentNullException(nameof(onFalse));
            (Flag, this.onFalse) = (flag, () => onFalse);
        }

        public Func<E> OnFalse =>
            onFalse ?? throw new InvalidOperationException("Guard isn't initialised. It was probably created via new Guard() or default(Guard), and so it has no OnFalse handler");
    }
}
