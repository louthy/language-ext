using System.Diagnostics.Contracts;
using System;
using LanguageExt.TypeClasses;
using LanguageExt.Instances;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;

namespace LanguageExt
{
    /// <summary>
    /// Discriminated union type.  Can be in one of two states:
    /// 
    ///     Some(a)
    ///     
    ///     None
    ///     
    /// </summary>
    /// <typeparam name="A">The type of the bound value</typeparam>
    internal abstract class OptionV<A>
    {
        /// <summary>
        /// None constructor for Option<A>
        /// </summary>
        public readonly static OptionV<A> None =
            new None<A>();

        /// <summary>
        /// Some(a) constructor for OptionV<A>
        /// </summary>
        /// <remarks>
        /// Will throw an exception if a is null
        /// </remarks>
        /// <param name="a">Value to construct the Option with</param>
        /// <returns>An Option in a Some state</returns>
        [Pure]
        public static OptionV<A> Some(A a) =>
            a == null
                ? failwith<OptionV<A>>("Option.Some() doesn't accept null")
                : new SomeValue<A>(a);

        /// <summary>
        /// Option constructor.  If a is null then the returned option
        /// is in a None state, otherwise it returns Some(a)
        /// </summary>
        /// <param name="a">Value to construct the option with</param>
        /// <returns>If a is null then the returned option
        /// is in a None state, otherwise it returns Some(a)</returns>
        [Pure]
        public static OptionV<A> Optional(A a) =>
            isnull(a)
                ? None
                : new SomeValue<A>(a);

        /// <summary>
        /// Internal cast of an OptionV<A> to a Some<A>
        /// </summary>
        /// <param name="ma">Option to cast</param>
        /// <returns>Some(a)</returns>
        [Pure]
        internal static SomeValue<A> Some(OptionV<A> ma) =>
            (SomeValue<A>)ma;
    }
}
