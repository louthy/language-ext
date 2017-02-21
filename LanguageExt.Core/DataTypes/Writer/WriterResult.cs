using LanguageExt.TypeClasses;

namespace LanguageExt
{
    /// <summary>
    /// Convenience methods for returning from a Writer<MonoidW, W, A> computation
    /// </summary>
    public static class WriterResult
    {
        public static (A Value, W Output, bool IsBottom) ToWriter<MonoidW, W, A>(this (A, W) self)
            where MonoidW : struct, Monoid<W> =>
                self.Add(false);

        public static (A Value, W Output, bool IsBottom) Return<MonoidW, W, A>(A value, W output)
            where MonoidW : struct, Monoid<W> =>
                (value, output, false);

        public static (Unit Value, W Output, bool IsBottom) Return<MonoidW, W>(W output)
            where MonoidW : struct, Monoid<W> =>
                (Unit.Default, output, false);

        public static (A Value, W Output, bool IsBottom) Fail<MonoidW, W, A>()
            where MonoidW : struct, Monoid<W> =>
                (default(A), default(W), true);

        public static (Unit Value, W Output, bool IsBottom) Fail<MonoidW, W>()
            where MonoidW : struct, Monoid<W> =>
                (default(Unit), default(W), true);
    }
}
