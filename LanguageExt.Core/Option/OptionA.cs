using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt
{
    /// <summary>
    /// Discriminated union type.  Can be in one of two states:
    /// 
    ///     Some(a)
    ///     
    ///     None
    ///     
    /// All instance methods are provided as extension methods.  This is to defend
    /// against null references, and to equate null with None.  Therefore you do not
    /// need to check for null when using Option
    /// </summary>
    /// <typeparam name="A">The type of the bound value</typeparam>
    public abstract class Option<A> : IOption<A>
    {
        /// <summary>
        /// None constructor for Option<A>
        /// </summary>
        public readonly static Option<A> None =
            new None<A>();

        /// <summary>
        /// Some(a) constructor for Option<A>
        /// </summary>
        /// <remarks>
        /// Will throw an exception if a is null
        /// </remarks>
        /// <param name="a">Value to construct the Option with</param>
        /// <returns>An Option in a Some state</returns>
        [Pure]
        public static Option<A> Some(A a) =>
            a == null
                ? failwith<Option<A>>("Option.Some() doesn't accept null")
                : new Some<A>(a);

        /// <summary>
        /// Option constructor.  If a is null then the returned option
        /// is in a None state, otherwise it returns Some(a)
        /// </summary>
        /// <param name="a">Value to construct the option with</param>
        /// <returns>If a is null then the returned option
        /// is in a None state, otherwise it returns Some(a)</returns>
        [Pure]
        public static Option<A> Optional(A a) =>
            isnull(a)
                ? None
                : new Some<A>(a);

        /// <summary>
        /// Internal cast of an Option<A> to a Some<A>
        /// </summary>
        /// <param name="ma">Option to cast</param>
        /// <returns>Some(a)</returns>
        [Pure]
        internal static Some<A> Some(Option<A> ma) =>
            (Some<A>)ma;

        /// <summary>
        /// Implicit conversion operator from A to Option<A>
        /// </summary>
        /// <remarks>null values of A will be coersced to None, Some(a) 
        /// otherwise</remarks>
        /// <param name="a">Value to be bound</param>
        [Pure]
        public static implicit operator Option<A>(A a) =>
            Optional(a);

        /// <summary>
        /// Implicit conversion operator from OptionNone to Option<A>
        /// </summary>
        /// <param name="none">None value</param>
        [Pure]
        public static implicit operator Option<A>(OptionNone none) =>
            None;

        /// <summary>
        /// Implicit conversion operator from Unit to Option<A>
        /// </summary>
        /// <param name="a">Unit value</param>
        [Pure]
        public static implicit operator Option<A>(Unit a) =>
            None;
    }
}
