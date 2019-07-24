using static LanguageExt.Prelude;
using LanguageExt.TypeClasses;
using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct MWriter<MonoidW, W, A> :
        MonadWriter<MonoidW, W, A>,
        Monad<Unit, (W, bool), Writer<MonoidW, W, A>, A>
        where MonoidW : struct, Monoid<W>
    {
        [Pure]
        public MB Bind<MONADB, MB, B>(Writer<MonoidW, W, A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, (W, bool), MB, B> =>
            default(MONADB).Run(_ =>
            {
                var (a, output1, faulted) = ma();
                return faulted
                    ? default(MONADB).Fail()
                    : default(MONADB).BindReturn((output1, faulted), f(a));
            });

        [Pure]
        public MB BindAsync<MONADB, MB, B>(Writer<MonoidW, W, A> ma, Func<A, MB> f) where MONADB : struct, MonadAsync<Unit, (W, bool), MB, B> =>
            default(MONADB).RunAsync(_ =>
            {
                var (a, output1, faulted) = ma();
                return faulted
                    ? default(MONADB).Fail().AsTask()
                    : default(MONADB).BindReturn((output1, faulted), f(a)).AsTask();
            });

        [Pure]
        public Writer<MonoidW, W, A> BindReturn((W, bool) output, Writer<MonoidW, W, A> mb)
        {
            var (b, output2, faulted) = mb();
            return () => faulted
                ? (default(A), default(MonoidW).Empty(), true)
                : (b, default(MonoidW).Append(output.Item1, output2), false);
        }

        [Pure]
        public Writer<MonoidW, W, A> Fail(object err = null) =>
            new Writer<MonoidW, W, A>(() => (default(A), default(MonoidW).Empty(), true));

        [Pure]
        public Writer<MonoidW, W, A> Writer(A value, W output) =>
            () => (value, output, false);

        [Pure]
        public Writer<MonoidW, W, A> Run(Func<Unit, Writer<MonoidW, W, A>> f) =>
            f(unit);

        [Pure]
        public Writer<MonoidW, W, A> Return(Func<Unit, A> f) =>
            () => (f(unit), default(MonoidW).Empty(), false);

        /// <summary>
        /// Tells the monad what you want it to hear.  The monad carries this 'packet'
        /// upwards, merging it if needed (hence the Monoid requirement).
        /// </summary>
        /// <typeparam name="W">Type of the value tell</typeparam>
        /// <param name="what">The value to tell</param>
        /// <returns>Updated writer monad</returns>
        [Pure]
        public Writer<MonoidW, W, Unit> Tell(W what) => () =>
            (unit, what, false);

        /// <summary>
        /// 'listen' is an action that executes the monad and adds
        /// its output to the value of the computation.
        /// </summary>
        [Pure]
        public Writer<MonoidW, W, (A, B)> Listen<B>(Writer<MonoidW, W, A> ma, Func<W, B> f) => () =>
        {
            var (a, output, faulted) = ma();
            if (faulted) return (default((A, B)), default(MonoidW).Empty(), true);
            return ((a, f(output)), output, false);
        };

        [Pure]
        public Writer<MonoidW, W, A> Plus(Writer<MonoidW, W, A> ma, Writer<MonoidW, W, A> mb) => () =>
        {
            var (a, output, faulted) = ma();
            return faulted
                ? mb()
                : (a, output, faulted);
        };

        [Pure]
        public Writer<MonoidW, W, A> Zero() =>
            () => (default(A), default(MonoidW).Empty(), true);

        [Pure]
        public Func<Unit, S> Fold<S>(Writer<MonoidW, W, A> fa, S state, Func<S, A, S> f) => _ =>
        {
            var (a, output, faulted) = fa();
            return faulted
                ? state
                : f(state, a);
        };

        [Pure]
        public Func<Unit, S> FoldBack<S>(Writer<MonoidW, W, A> fa, S state, Func<S, A, S> f) =>
            Fold(fa, state, f);

        [Pure]
        public Func<Unit, int> Count(Writer<MonoidW, W, A> fa) =>
            Fold(fa, 0, (s,x) => 1);

        [Pure]
        public Writer<MonoidW, W, A> Apply(Func<A, A, A> f, Writer<MonoidW, W, A> fa, Writer<MonoidW, W, A> fb) =>
            from a in fa
            from b in fb
            select f(a, b);
    }
}
