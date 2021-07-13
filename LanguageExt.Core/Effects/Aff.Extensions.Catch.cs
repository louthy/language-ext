using System;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Catch an error if the predicate matches
        /// </summary>
        public static AffCatch<A> Catch<A>(Func<Error, bool> predicate, Func<Error, Aff<A>> Fail) =>
            new AffCatch<A>(predicate, Fail);

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static AffCatch<A> CatchEx<A>(Func<Exception, bool> predicate, Func<Exception, Aff<A>> Fail) =>
            Catch(e => e.Exception.Map(predicate).IfNone(false), e => Fail(e));

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static AffCatch<A> CatchEx<A>(Func<Exception, bool> predicate, Aff<A> Fail) =>
            Catch(e => e.Exception.Map(predicate).IfNone(false), e => Fail);

        /// <summary>
        /// Catch an error if the predicate matches
        /// </summary>
        public static AffCatch<RT, A> Catch<RT, A>(Func<Error, bool> predicate, Func<Error, Aff<RT, A>> Fail) where RT : struct, HasCancel<RT> =>
            new AffCatch<RT, A>(predicate, Fail);

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static AffCatch<RT, A> CatchEx<RT, A>(Func<Exception, bool> predicate, Func<Exception, Aff<RT, A>> Fail) where RT : struct, HasCancel<RT> =>
            Catch(e => e.Exception.Map(predicate).IfNone(false), e => Fail(e));

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static AffCatch<RT, A> CatchEx<RT, A>(Func<Exception, bool> predicate, Aff<RT, A> Fail) where RT : struct, HasCancel<RT> =>
            Catch(e => e.Exception.Map(predicate).IfNone(false), e => Fail);
    }
}
