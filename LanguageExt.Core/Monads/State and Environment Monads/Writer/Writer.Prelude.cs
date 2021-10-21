using System;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Writer monad constructor
        /// </summary>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>Writer monad</returns>
        [Pure]
        public static Writer<MonoidW, W, A> Writer<MonoidW, W, A>(A value) where MonoidW : struct, Monoid<W> => () => 
            (value, default(MonoidW).Empty(), false);

        /// <summary>
        /// Writer monad constructor
        /// </summary>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="output">Output</param>
        /// <param name="value">Value</param>
        /// <returns>Writer monad</returns>
        [Pure]
        public static Writer<MonoidW, W, A> Writer<MonoidW, W, A>(A value, W output) where MonoidW : struct, Monoid<W> => () =>
            (value, output, false);

        /// <summary>
        /// Writer monad constructor
        /// </summary>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>Writer monad</returns>
        [Pure]
        public static Writer<MSeq<W>, Seq<W>, A> Writer<W, A>(A value) => () =>
            (value, default(MSeq<W>).Empty(), false);

        /// <summary>
        /// Writer monad constructor
        /// </summary>
        /// <typeparam name="W">Writer type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="output">Output</param>
        /// <param name="value">Value</param>
        /// <returns>Writer monad</returns>
        [Pure]
        public static Writer<MSeq<W>, Seq<W>, A> Writer<W, A>(A value, Seq<W> output) => () =>
            (value, output, false);

        /// <summary>
        /// Count
        /// </summary>
        [Pure]
        public static Writer<MonoidW, W, int> count<MonoidW, W>(Writer<MonoidW, W, int> self)
            where MonoidW : struct, Monoid<W> =>
                self.Count();

        /// <summary>
        /// For all
        /// </summary>
        [Pure]
        public static Writer<MonoidW, W, bool> forall<MonoidW, W, A>(Writer<MonoidW, W, A> self, Func<A, bool> pred)
            where MonoidW : struct, Monoid<W> =>
                self.ForAll(pred);

        /// <summary>
        /// Exists
        /// </summary>
        [Pure]
        public static Writer<MonoidW, W, bool> exists<MonoidW, W, A>(Writer<MonoidW, W, A> self, Func<A, bool> pred)
            where MonoidW : struct, Monoid<W> =>
                self.Exists(pred);

        /// <summary>
        /// Fold
        /// </summary>
        [Pure]
        public static Writer<MonoidW, W, FState> fold<FState, MonoidW, W, A>(Writer<MonoidW, W, A> self, FState initialState, Func<FState, A, FState> f)
            where MonoidW : struct, Monoid<W> =>
                self.Fold(initialState, f);

        /// <summary>
        /// Fold
        /// </summary>
        [Pure]
        public static Writer<MonoidW, W, W> fold<MonoidW, W, A>(Writer<MonoidW, W, A> self, Func<W, A, W> f)
            where MonoidW : struct, Monoid<W> =>
                self.Fold(f);

        /// <summary>
        /// pass is an action that executes the monad, which
        /// returns a value and a function, and returns the value, applying
        /// the function to the output.
        /// </summary>
        [Pure]
        public static Writer<MonoidW, W, A> pass<MonoidW, W, A>(Writer<MonoidW, W, (A, Func<W, W>)> self)
            where MonoidW : struct, Monoid<W> =>
                self.Pass();

        /// <summary>
        /// listen is an action that executes the monad and adds
        /// its output to the value of the computation.
        /// </summary>
        [Pure]
        public static Writer<MonoidW, W, (A, B)> listen<MonoidW, W, A, B>(Writer<MonoidW, W, A> self, Func<W, B> f)
            where MonoidW : struct, Monoid<W> =>
                self.Listen(f);

        /// <summary>
        /// Censor is an action that executes the writer monad and applies the function f 
        /// to its output, leaving the return value unchanged.
        /// </summary>
        public static Writer<MonoidW, W, A> censor<MonoidW, W, A>(Writer<MonoidW, W, A> self, Func<W, W> f)
            where MonoidW : struct, Monoid<W> =>
                self.Censor(f);

        /// <summary>
        /// Tells the monad what you want it to hear.  The monad carries this 'packet'
        /// upwards, merging it if needed (hence the Monoid requirement).
        /// </summary>
        /// <typeparam name="W">Type of the value tell</typeparam>
        /// <param name="what">The value to tell</param>
        /// <returns>Updated writer monad</returns>
        [Pure]
        public static Writer<MonoidW, W, Unit> tell<MonoidW, W>(W what)
            where MonoidW : struct, Monoid<W> =>
                default(MWriter<MonoidW, W, Unit>).Tell(what);

        /// <summary>
        /// Tells the monad what you want it to hear.  The monad carries this 'packet'
        /// upwards, merging it if needed (hence the Monoid requirement).
        /// </summary>
        /// <typeparam name="W">Type of the value tell</typeparam>
        /// <param name="what">The value to tell</param>
        /// <returns>Updated writer monad</returns>
        [Pure]
        public static Writer<MSeq<W>, Seq<W>, Unit> tell<W>(W what) =>
            default(MWriter<MSeq<W>, Seq<W>, Unit>).Tell(Seq1(what));

        /// <summary>
        /// Bind
        /// </summary>
        [Pure]
        public static Writer<MonoidW, W, B> bind<MonoidW, W, A, B>(Writer<MonoidW, W, A> self, Func<A, Writer<MonoidW, W, B>> f)
            where MonoidW : struct, Monoid<W> =>
                self.Bind(f);

        /// <summary>
        /// Map
        /// </summary>
        [Pure]
        public static Writer<MonoidW, W, B> map<MonoidW, W, A, B>(Writer<MonoidW, W, A> self, Func<A, B> f)
            where MonoidW : struct, Monoid<W> =>
                self.Map(f);

        /// <summary>
        /// Filter
        /// </summary>
        [Pure]
        public static Writer<MonoidW, W, A> filter<MonoidW, W, A>(Writer<MonoidW, W, A> self, Func<A, bool> pred)
            where MonoidW : struct, Monoid<W> =>
                self.Filter(pred);

        /// <summary>
        /// Iter
        /// </summary>
        public static Writer<MonoidW, W, Unit> iter<MonoidW, W, A>(Writer<MonoidW, W, A> self, Action<A> action)
            where MonoidW : struct, Monoid<W> =>
                self.Iter(action);

        /// <summary>
        /// Run the writer and catch exceptions
        /// </summary>
        [Pure]
        public static Writer<MonoidW, W, A> trywrite<MonoidW, W, A>(Writer<MonoidW, W, A> m) 
            where MonoidW : struct, Monoid<W> =>
                () =>
                {
                    try
                    {
                        return m();
                    }
                    catch
                    {
                        return (default(A), default(MonoidW).Empty(), true);
                    }
                };

        [Pure]
        public static Try<Writer<MonoidW, W, A>> tryfun<MonoidW, W, A>(Writer<MonoidW, W, A> ma) 
            where MonoidW : struct, Monoid<W> => () =>
                from x in ma
                select x;
    }
}
