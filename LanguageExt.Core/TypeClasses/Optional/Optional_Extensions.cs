using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class TypeClassExtensions
    {
        /// <summary>
        /// Fluent pattern matching.  Provide a Some handler and then follow
        /// on fluently with .None(...) to complete the matching operation.
        /// This is for dispatching actions, use Some<A,B>(...) to return a value
        /// from the match operation.
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ma">Option to match</param>
        /// <param name="f">The Some(x) match operation</param>
        [Pure]
        public static SomeUnitContext<OPT, A> Some<OPT, A>(this OPT ma, Action<A> f) where OPT : struct, Optional<A> =>
            new SomeUnitContext<OPT, A>(ma, f, default(OPT).IsUnsafe(ma));

        /// <summary>
        /// Fluent pattern matching.  Provide a Some handler and then follow
        /// on fluently with .None(...) to complete the matching operation.
        /// This is for returning a value from the match operation, to dispatch
        /// an action instead, use Some<A>(...)
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <typeparam name="B">Match operation return value type</typeparam>
        /// <param name="ma">Option to match</param>
        /// <param name="f">The Some(x) match operation</param>
        /// <returns>The result of the match operation</returns>
        [Pure]
        public static SomeContext<OPT, A, B> Some<OPT, A, B>(this Optional<A> ma, Func<A, B> f) where OPT : struct, Optional<A> =>
            new SomeContext<OPT, A, B>(ma, f, default(OPT).IsUnsafe(ma));

        public static Unit Match<OPT, A>(this Optional<A> ma, Action<A> Some, Action None) where OPT : struct, Optional<A> =>
            default(OPT).IsUnsafe(ma)
                ? default(OPT).MatchUnsafe(ma,
                    Some: a  => { Some(a); return unit; },
                    None: () => { return unit; })
                : default(OPT).Match(ma,
                    Some: a  => { Some(a); return unit; },
                    None: () => { return unit; });

        public static Unit IfSome<OPT, A>(this Optional<A> ma, Action<A> f) where OPT : struct, Optional<A> =>
            default(OPT).IsUnsafe(ma)
                ? default(OPT).MatchUnsafe(ma,
                    Some: a => { f(a); return unit; },
                    None: () => { return unit; })
                : default(OPT).Match(ma,
                    Some: a  => { f(a); return unit; },
                    None: () => { return unit; });

        [Pure]
        public static A IfNone<OPT, A>(this Optional<A> ma, Func<A> None) where OPT : struct, Optional<A> =>
            default(OPT).IsUnsafe(ma)
                ? default(OPT).MatchUnsafe(ma,
                    Some: a => a,
                    None: None)
                : default(OPT).Match(ma,
                    Some: a  => a,
                    None: None);

        [Pure]
        public static A IfNone<OPT, A>(this Optional<A> ma, A noneValue) where OPT : struct, Optional<A> =>
            default(OPT).IsUnsafe(ma)
                ? default(OPT).MatchUnsafe(ma,
                    Some: a => a,
                    None: () => noneValue)
                : default(OPT).Match(ma,
                    Some: a  => a,
                    None: () => noneValue);

        [Pure]
        public static A IfNoneUnsafe<OPT, A>(this Optional<A> ma, Func<A> None) where OPT : struct, Optional<A> =>
            default(OPT).MatchUnsafe(ma,
                Some: a  =>  a,
                None: None
                );

        [Pure]
        public static A IfNoneUnsafe<OPT, A>(this Optional<A> ma, A noneValue) where OPT : struct, Optional<A> =>
            default(OPT).MatchUnsafe(ma,
                Some: a  => a,
                None: () => noneValue
                );

    }
}
