using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Option Some state type
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    public class SomeValue<A> : Option<A>
    {
        /// <summary>
        /// The bound value of the option
        /// </summary>
        public readonly A Value;

        internal SomeValue(A value)
        {
            Value = value;
        }

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
