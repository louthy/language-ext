using System;
using LanguageExt.Common;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Catch an error if the predicate matches
        /// </summary>
        public static EffCatch<A> Catch<A>(Func<Error, bool> predicate, Func<Error, Eff<A>> Fail) =>
            new EffCatch<A>(predicate, Fail);

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static EffCatch<A> CatchEx<A>(Func<Exception, bool> predicate, Func<Exception, Eff<A>> Fail) =>
            Catch(e => e.Exception.Map(predicate).IfNone(false), e => Fail(e));

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static EffCatch<A> CatchEx<A>(Func<Exception, bool> predicate, Eff<A> Fail) =>
            Catch(e => e.Exception.Map(predicate).IfNone(false), e => Fail);
-
        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static EffCatch<A> CatchEx<A>(Func<Exception, bool> predicate, A Fail) =>
            Catch(e => e.Exception.Map(predicate).IfNone(false), e => Fail);

        /// <summary>
        /// Catch an error if the predicate matches
        /// </summary>
        public static EffCatch<A> Catch<A>(Func<Error, bool> predicate, Func<Error, A> Fail) =>
            Catch(predicate, e => SuccessEff(Fail(e)));

        /// <summary>
        /// Catch an error if the predicate matches
        /// </summary>
        public static EffCatch<A> Catch<A>(int errorCode, Func<Error, A> Fail) =>
            Catch(e => e.Code == errorCode, e => SuccessEff(Fail(e)));

        /// <summary>
        /// Catch an error if the predicate matches
        /// </summary>
        public static EffCatch<A> Catch<A>(Func<Error, bool> predicate, A Fail) =>
            Catch(predicate, e => SuccessEff(Fail));

        /// <summary>
        /// Catch an error if the predicate matches
        /// </summary>
        public static EffCatch<A> Catch<A>(int errorCode, A Fail) =>
            Catch(e => e.Code == errorCode, e => SuccessEff(Fail));

        /// <summary>
        /// Catch an error if the predicate matches
        /// </summary>
        public static EffCatch<RT, A> Catch<RT, A>(Func<Error, bool> predicate, Func<Error, Eff<RT, A>> Fail) =>
            new EffCatch<RT, A>(predicate, Fail);

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static EffCatch<RT, A> CatchEx<RT, A>(Func<Exception, bool> predicate, Func<Exception, Eff<RT, A>> Fail) =>
            Catch(e => e.Exception.Map(predicate).IfNone(false), e => Fail(e));

        /// <summary>
        /// Catch an error if it's of a specific exception type
        /// </summary>
        public static EffCatch<RT, A> CatchEx<RT, A>(Func<Exception, bool> predicate, Eff<RT, A> Fail) =>
            Catch(e => e.Exception.Map(predicate).IfNone(false), e => Fail);
    }
}
