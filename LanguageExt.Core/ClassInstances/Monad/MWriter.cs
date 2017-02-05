using static LanguageExt.Prelude;
using LanguageExt.TypeClasses;
using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct MWriter<SWriterA, SWA, MonoidW, W, A> : MonadWriter<SWriterA, SWA, MonoidW, W, A>, Monad<W, SWA, A>
        where SWriterA : struct, WriterMonadValue<SWA, W, A>
        where MonoidW  : struct, Monoid<W>
    {
        [Pure]
        public MB Bind<MONADB, MB, B>(SWA ma, Func<A, MB> f) where MONADB : struct, Monad<W, MB, B> =>
            default(MONADB).Return(write1 =>
            {
                var (x, write2, bottom) = default(SWriterA).Eval(ma, write1);
                if (bottom) return (default(B), write1, true);
                return default(MONADB).Eval(f(x), write2);
            });

        [Pure]
        public (A, W, bool) Eval(SWA ma, W output) =>
            default(SWriterA).Eval(ma, output);

        [Pure]
        public SWA Fail(Exception err = null) =>
            default(SWriterA).Bottom;

        [Pure]
        public SWA Fail(object err) =>
            default(SWriterA).Bottom;

        [Pure]
        public SWA Writer(A value, W output) =>
            default(SWriterA).Lift(w => (value, default(MonoidW).Append(w, output), false));

        [Pure]
        public SWA Return(A x) =>
            Writer(x, default(MonoidW).Empty());

        [Pure]
        public SWA Return(Func<W, (A, W, bool)> f) =>
            default(SWriterA).Lift(f);

        /// <summary>
        /// Tells the monad what you want it to hear.  The monad carries this 'packet'
        /// upwards, merging it if needed (hence the Monoid requirement).
        /// </summary>
        /// <typeparam name="W">Type of the value tell</typeparam>
        /// <param name="what">The value to tell</param>
        /// <returns>Updated writer monad</returns>
        [Pure]
        public SSU Tell<SWriterU, SSU>(W what)
            where SWriterU : struct, WriterMonadValue<SSU, W, Unit> =>
                default(SWriterU).Lift(w => (unit, default(MonoidW).Append(w, what), false));

        /// <summary>
        /// 'listen' is an action that executes the monad and adds
        /// its output to the value of the computation.
        /// </summary>
        [Pure]
        public SSAW Listen<SWriterAW, SSAW>(SWA ma)
            where SWriterAW : struct, WriterMonadValue<SSAW, W, (A, W)> =>
                default(SWriterAW).Lift(written =>
                {
                    var (a, w, b) = default(SWriterA).Eval(ma, written);
                    return b
                        ? (default((A, W)), default(MonoidW).Empty(), true)
                        : ((a, w), w, false);
                });
    }
}
