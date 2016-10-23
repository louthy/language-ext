using System;

namespace LanguageExt
{
    /// <summary>
    /// Option None state type
    /// </summary>
    /// <typeparam name="A">Bound value type - not used for None</typeparam>
    internal class None<A> : OptionV<A>
    {
        public static readonly OptionV<A> Default = new None<A>();

        internal None()
        { }

        public override bool IsSome => false;

        public override A Value => 
            Prelude.raise<A>(new ValueIsNoneException());

        public override string ToString() =>
            $"None";

        public override int GetHashCode() =>
            0;
    }
}
