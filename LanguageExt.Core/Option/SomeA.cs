using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Option Some state type
    /// </summary>
    /// <typeparam name="A">Type of the bound value</typeparam>
    public class Some<A> : Option<A>
    {
        /// <summary>
        /// The bound value of the option
        /// </summary>
        public readonly A Value;

        internal Some(A value)
        {
            Value = value;
        }

        public static implicit operator Some<A>(A value) =>
            value == null
                ? failwith<Some<A>>("Some cannot be null")
                : new Some<A>(value);
    }
}
