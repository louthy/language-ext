using LanguageExt.TypeClasses;
using System;

namespace LanguageExt.ClassInstances
{
    public struct SWriter<MonoidW, W, A> : WriterMonadValue<Writer<MonoidW, W, A>, W, A>
        where MonoidW : struct, Monoid<W>
    {
        static readonly Writer<MonoidW, W, A> bottom = new Writer<MonoidW, W, A>(_ => (default(A), default(MonoidW).Empty(), true));

        public (A, W, bool) Eval(Writer<MonoidW, W, A> r, W env) =>
            r.eval(env);

        public Writer<MonoidW, W, A> Lift((A, W, bool) value) =>
            new Writer<MonoidW, W, A>(_ => value);

        public Writer<MonoidW, W, A> Lift(Func<W, (A, W, bool)> f) =>
            new Writer<MonoidW, W, A>(f);

        public Writer<MonoidW, W, A> Bottom => bottom;
    }
}