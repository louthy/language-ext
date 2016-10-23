using System;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Option Some state type
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    internal class SomeValue<A> : OptionV<A>
    {
        /// <summary>
        /// The bound value of the option
        /// </summary>
        readonly A value;

        public override bool IsSome => true;

        internal SomeValue(A value)
        {
            this.value = value;
        }

        public override A Value =>
            value;

        public static implicit operator SomeValue<A>(A value) =>
            value == null
                ? failwith<SomeValue<A>>("Some cannot be null")
                : new SomeValue<A>(value);

        public override string ToString() =>
            $"Some({Value})";

        public override int GetHashCode() =>
            Value.GetHashCode();
    }
}
