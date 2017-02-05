using LanguageExt.TypeClasses;
using System;

namespace LanguageExt.ClassInstances
{
    public struct SRWS<MonoidW, R, W, S, A> :
        MonadValue<RWS<MonoidW, R, W, S, A>, (R, W, S), A>,
        ReaderMonadValue<RWS<MonoidW, R, W, S, A>, R, A>,
        WriterMonadValue<RWS<MonoidW, R, W, S, A>, W, A>,
        StateMonadValue<RWS<MonoidW, R, W, S, A>, S, A>
        where MonoidW : struct, Monoid<W>
    {
        static readonly (A, (R, W, S), bool) bottomv = (default(A), (default(R), default(MonoidW).Empty(), default(S)), true);
        static readonly RWS<MonoidW, R, W, S, A> bottom = new RWS<MonoidW, R, W, S, A>(_ => bottomv);

        public RWS<MonoidW, R, W, S, A> Bottom => bottom;

        public (A, (R, W, S), bool) Eval(RWS<MonoidW, R, W, S, A> ma, (R, W, S) state) =>
            ma.eval(state);

        public RWS<MonoidW, R, W, S, A> Lift((A, (R, W, S), bool) value) =>
            new RWS<MonoidW, R, W, S, A>(_ => value);

        public RWS<MonoidW, R, W, S, A> Lift(Func<(R, W, S), (A, (R, W, S), bool)> f) =>
            new RWS<MonoidW, R, W, S, A>(f);

        public (A, R, bool) Eval(RWS<MonoidW, R, W, S, A> ma, R state)
        {
            var (a, (r, w, s), b) = ma.eval((state, default(MonoidW).Empty(), default(S)));
            return (a, state, b);
        }

        public RWS<MonoidW, R, W, S, A> Lift((A, R, bool) value) =>
            new RWS<MonoidW, R, W, S, A>(rws => (value.Item1, (value.Item2, rws.Item2, rws.Item3), false));

        public RWS<MonoidW, R, W, S, A> Lift(Func<R, (A, R, bool)> f) =>
            new RWS<MonoidW, R, W, S, A>(rws =>
            {
                var (a, r, b) = f(rws.Item1);
                return (a, (r, rws.Item2, rws.Item3), b);
            });

        public (A, W, bool) Eval(RWS<MonoidW, R, W, S, A> ma, W output)
        {
            var (a, (r, w, s), b) = ma.eval((default(R), output, default(S)));
            return (a, default(MonoidW).Append(output, w), b);
        }

        public RWS<MonoidW, R, W, S, A> Lift((A, W, bool) value) =>
            new RWS<MonoidW, R, W, S, A>(rws => (value.Item1, (rws.Item1, value.Item2, rws.Item3), false));

        public RWS<MonoidW, R, W, S, A> Lift(Func<W, (A, W, bool)> f) =>
            new RWS<MonoidW, R, W, S, A>(rws =>
            {
                var(a, w, b) = f(rws.Item2);
                return (a, (rws.Item1, default(MonoidW).Append(rws.Item2, w), rws.Item3), b);
            });

        public (A, S, bool) Eval(RWS<MonoidW, R, W, S, A> ma, S state)
        {
            var (a, (r, w, s), b) = ma.eval((default(R), default(MonoidW).Empty(), state));
            return (a, s, b);
        }

        public RWS<MonoidW, R, W, S, A> Lift((A, S, bool) value) =>
            new RWS<MonoidW, R, W, S, A>(rws => (value.Item1, (rws.Item1, rws.Item2, value.Item2), false));

        public RWS<MonoidW, R, W, S, A> Lift(Func<S, (A, S, bool)> f) =>
            new RWS<MonoidW, R, W, S, A>(rws =>
            {
                var (a, s, b) = f(rws.Item3);
                return (a, (rws.Item1, rws.Item2, s), b);
            });
    }
}