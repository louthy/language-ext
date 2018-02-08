
using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class WriterT
    {
        public static Writer<MonoidW, W, Seq<A>> sequence<MonoidW, W, A>(Seq<Writer<MonoidW, W, A>> ma) where MonoidW : struct, Monoid<W> =>
            SeqWriterExtensions.SequenceFast(ma).Map(Prelude.Seq);

        public static Writer<MonoidW, W, Lst<A>> sequence<MonoidW, W, A>(Lst<Writer<MonoidW, W, A>> ma) where MonoidW : struct, Monoid<W> =>
            SeqWriterExtensions.SequenceFast(ma).Map(toList);

        public static Writer<MonoidW, W, Arr<A>> sequence<MonoidW, W, A>(Arr<Writer<MonoidW, W, A>> ma) where MonoidW : struct, Monoid<W> =>
            SeqWriterExtensions.SequenceFast(ma).Map(toArray);

        public static Writer<MonoidW, W, A[]> sequence<MonoidW, W, A>(Writer<MonoidW, W, A>[] ma) where MonoidW : struct, Monoid<W> =>
            SeqWriterExtensions.SequenceFast(ma).Map(x => x.ToArray());

        public static Writer<MonoidW, W, Set<A>> sequence<MonoidW, W, A>(Set<Writer<MonoidW, W, A>> ma) where MonoidW : struct, Monoid<W> =>
            SeqWriterExtensions.SequenceFast(ma).Map(toSet);

        public static Writer<MonoidW, W, HashSet<A>> sequence<MonoidW, W, A>(HashSet<Writer<MonoidW, W, A>> ma) where MonoidW : struct, Monoid<W> =>
            SeqWriterExtensions.SequenceFast(ma).Map(toHashSet);

        public static Writer<MonoidW, W, Stck<A>> sequence<MonoidW, W, A>(Stck<Writer<MonoidW, W, A>> ma) where MonoidW : struct, Monoid<W> =>
            SeqWriterExtensions.SequenceFast(ma).Map(toStack);

        public static Writer<MonoidW, W, IEnumerable<A>> sequence<MonoidW, W, A>(IEnumerable<Writer<MonoidW, W, A>> ma) where MonoidW : struct, Monoid<W> =>
            SeqWriterExtensions.SequenceFast(ma).Map(Enumerable.AsEnumerable);


        public static Writer<MonoidW, W, Seq<B>> traverse<MonoidW, W, A, B>(Seq<Writer<MonoidW, W, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            SeqWriterExtensions.TraverseFast(ma, f).Map(Prelude.Seq);

        public static Writer<MonoidW, W, Lst<B>> traverse<MonoidW, W, A, B>(Lst<Writer<MonoidW, W, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            SeqWriterExtensions.TraverseFast(ma, f).Map(toList);

        public static Writer<MonoidW, W, Arr<B>> traverse<MonoidW, W, A, B>(Arr<Writer<MonoidW, W, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            SeqWriterExtensions.TraverseFast(ma, f).Map(toArray);

        public static Writer<MonoidW, W, B[]> traverse<MonoidW, W, A, B>(Writer<MonoidW, W, A>[] ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            SeqWriterExtensions.TraverseFast(ma, f).Map(x => x.ToArray());

        public static Writer<MonoidW, W, Set<B>> traverse<MonoidW, W, A, B>(Set<Writer<MonoidW, W, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            SeqWriterExtensions.TraverseFast(ma, f).Map(toSet);

        public static Writer<MonoidW, W, HashSet<B>> traverse<MonoidW, W, A, B>(HashSet<Writer<MonoidW, W, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            SeqWriterExtensions.TraverseFast(ma, f).Map(toHashSet);

        public static Writer<MonoidW, W, Stck<B>> traverse<MonoidW, W, A, B>(Stck<Writer<MonoidW, W, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            SeqWriterExtensions.TraverseFast(ma, f).Map(toStack);

        public static Writer<MonoidW, W, IEnumerable<B>> traverse<MonoidW, W, A, B>(IEnumerable<Writer<MonoidW, W, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            SeqWriterExtensions.TraverseFast(ma, f).Map(Enumerable.AsEnumerable);
    }

    public static partial class ReaderT
    {
        public static Reader<Env, Seq<A>> sequence<Env, A>(Seq<Reader<Env, A>> ma) =>
            SeqReaderExtensions.SequenceFast(ma).Map(Prelude.Seq);

        public static Reader<Env, Lst<A>> sequence<Env, A>(Lst<Reader<Env, A>> ma) =>
            SeqReaderExtensions.SequenceFast(ma).Map(toList);

        public static Reader<Env, Arr<A>> sequence<Env, A>(Arr<Reader<Env, A>> ma) =>
            SeqReaderExtensions.SequenceFast(ma).Map(toArray);

        public static Reader<Env, A[]> sequence<Env, A>(Reader<Env, A>[] ma) =>
            SeqReaderExtensions.SequenceFast(ma).Map(x => x.ToArray());

        public static Reader<Env, Set<A>> sequence<Env, A>(Set<Reader<Env, A>> ma) =>
            SeqReaderExtensions.SequenceFast(ma).Map(toSet);

        public static Reader<Env, HashSet<A>> sequence<Env, A>(HashSet<Reader<Env, A>> ma) =>
            SeqReaderExtensions.SequenceFast(ma).Map(toHashSet);

        public static Reader<Env, Stck<A>> sequence<Env, A>(Stck<Reader<Env, A>> ma) =>
            SeqReaderExtensions.SequenceFast(ma).Map(toStack);

        public static Reader<Env, IEnumerable<A>> sequence<Env, A>(IEnumerable<Reader<Env, A>> ma) =>
            SeqReaderExtensions.SequenceFast(ma).Map(Enumerable.AsEnumerable);


        public static Reader<Env, Seq<B>> traverse<Env, A, B>(Seq<Reader<Env, A>> ma, Func<A, B> f) =>
            SeqReaderExtensions.TraverseFast(ma, f).Map(Prelude.Seq);

        public static Reader<Env, Lst<B>> traverse<Env, A, B>(Lst<Reader<Env, A>> ma, Func<A, B> f) =>
            SeqReaderExtensions.TraverseFast(ma, f).Map(toList);

        public static Reader<Env, Arr<B>> traverse<Env, A, B>(Arr<Reader<Env, A>> ma, Func<A, B> f) =>
            SeqReaderExtensions.TraverseFast(ma, f).Map(toArray);

        public static Reader<Env, B[]> traverse<Env, A, B>(Reader<Env, A>[] ma, Func<A, B> f) =>
            SeqReaderExtensions.TraverseFast(ma, f).Map(x => x.ToArray());

        public static Reader<Env, Set<B>> traverse<Env, A, B>(Set<Reader<Env, A>> ma, Func<A, B> f) =>
            SeqReaderExtensions.TraverseFast(ma, f).Map(toSet);

        public static Reader<Env, HashSet<B>> traverse<Env, A, B>(HashSet<Reader<Env, A>> ma, Func<A, B> f) =>
            SeqReaderExtensions.TraverseFast(ma, f).Map(toHashSet);

        public static Reader<Env, Stck<B>> traverse<Env, A, B>(Stck<Reader<Env, A>> ma, Func<A, B> f) =>
            SeqReaderExtensions.TraverseFast(ma, f).Map(toStack);

        public static Reader<Env, IEnumerable<B>> traverse<Env, A, B>(IEnumerable<Reader<Env, A>> ma, Func<A, B> f) =>
            SeqReaderExtensions.TraverseFast(ma, f).Map(Enumerable.AsEnumerable);
    }

    public static partial class StateT
    {
        public static State<S, Seq<A>> sequence<S, A>(Seq<State<S, A>> ma) =>
            SeqStateExtensions.SequenceFast(ma).Map(Prelude.Seq);

        public static State<S, Lst<A>> sequence<S, A>(Lst<State<S, A>> ma) =>
            SeqStateExtensions.SequenceFast(ma).Map(toList);

        public static State<S, Arr<A>> sequence<S, A>(Arr<State<S, A>> ma) =>
            SeqStateExtensions.SequenceFast(ma).Map(toArray);

        public static State<S, A[]> sequence<S, A>(State<S, A>[] ma) =>
            SeqStateExtensions.SequenceFast(ma).Map(x => x.ToArray());

        public static State<S, Set<A>> sequence<S, A>(Set<State<S, A>> ma) =>
            SeqStateExtensions.SequenceFast(ma).Map(toSet);

        public static State<S, HashSet<A>> sequence<S, A>(HashSet<State<S, A>> ma) =>
            SeqStateExtensions.SequenceFast(ma).Map(toHashSet);

        public static State<S, Stck<A>> sequence<S, A>(Stck<State<S, A>> ma) =>
            SeqStateExtensions.SequenceFast(ma).Map(toStack);

        public static State<S, IEnumerable<A>> sequence<S, A>(IEnumerable<State<S, A>> ma) =>
            SeqStateExtensions.SequenceFast(ma).Map(Enumerable.AsEnumerable);


        public static State<S, Seq<B>> traverse<S, A, B>(Seq<State<S, A>> ma, Func<A, B> f) =>
            SeqStateExtensions.TraverseFast(ma, f).Map(Prelude.Seq);

        public static State<S, Lst<B>> traverse<S, A, B>(Lst<State<S, A>> ma, Func<A, B> f) =>
            SeqStateExtensions.TraverseFast(ma, f).Map(toList);

        public static State<S, Arr<B>> traverse<S, A, B>(Arr<State<S, A>> ma, Func<A, B> f) =>
            SeqStateExtensions.TraverseFast(ma, f).Map(toArray);

        public static State<S, B[]> traverse<S, A, B>(State<S, A>[] ma, Func<A, B> f) =>
            SeqStateExtensions.TraverseFast(ma, f).Map(x => x.ToArray());

        public static State<S, Set<B>> traverse<S, A, B>(Set<State<S, A>> ma, Func<A, B> f) =>
            SeqStateExtensions.TraverseFast(ma, f).Map(toSet);

        public static State<S, HashSet<B>> traverse<S, A, B>(HashSet<State<S, A>> ma, Func<A, B> f) =>
            SeqStateExtensions.TraverseFast(ma, f).Map(toHashSet);

        public static State<S, Stck<B>> traverse<S, A, B>(Stck<State<S, A>> ma, Func<A, B> f) =>
            SeqStateExtensions.TraverseFast(ma, f).Map(toStack);

        public static State<S, IEnumerable<B>> traverse<S, A, B>(IEnumerable<State<S, A>> ma, Func<A, B> f) =>
            SeqStateExtensions.TraverseFast(ma, f).Map(Enumerable.AsEnumerable);
    }

    public static partial class RwsT
    {
        public static RWS<MonoidW, R, W, S, Seq<A>> sequence<MonoidW, R, W, S, A>(Seq<RWS<MonoidW, R, W, S, A>> ma) where MonoidW : struct, Monoid<W> =>
            SeqRwsExtensions.SequenceFast(ma).Map(Prelude.Seq);

        public static RWS<MonoidW, R, W, S, Lst<A>> sequence<MonoidW, R, W, S, A>(Lst<RWS<MonoidW, R, W, S, A>> ma) where MonoidW : struct, Monoid<W> =>
            SeqRwsExtensions.SequenceFast(ma).Map(toList);

        public static RWS<MonoidW, R, W, S, Arr<A>> sequence<MonoidW, R, W, S, A>(Arr<RWS<MonoidW, R, W, S, A>> ma) where MonoidW : struct, Monoid<W> =>
            SeqRwsExtensions.SequenceFast(ma).Map(toArray);

        public static RWS<MonoidW, R, W, S, A[]> sequence<MonoidW, R, W, S, A>(RWS<MonoidW, R, W, S, A>[] ma) where MonoidW : struct, Monoid<W> =>
            SeqRwsExtensions.SequenceFast(ma).Map(x => x.ToArray());

        public static RWS<MonoidW, R, W, S, Set<A>> sequence<MonoidW, R, W, S, A>(Set<RWS<MonoidW, R, W, S, A>> ma) where MonoidW : struct, Monoid<W> =>
            SeqRwsExtensions.SequenceFast(ma).Map(toSet);

        public static RWS<MonoidW, R, W, S, HashSet<A>> sequence<MonoidW, R, W, S, A>(HashSet<RWS<MonoidW, R, W, S, A>> ma) where MonoidW : struct, Monoid<W> =>
            SeqRwsExtensions.SequenceFast(ma).Map(toHashSet);

        public static RWS<MonoidW, R, W, S, Stck<A>> sequence<MonoidW, R, W, S, A>(Stck<RWS<MonoidW, R, W, S, A>> ma) where MonoidW : struct, Monoid<W> =>
            SeqRwsExtensions.SequenceFast(ma).Map(toStack);

        public static RWS<MonoidW, R, W, S, IEnumerable<A>> sequence<MonoidW, R, W, S, A>(IEnumerable<RWS<MonoidW, R, W, S, A>> ma) where MonoidW : struct, Monoid<W> =>
            SeqRwsExtensions.SequenceFast(ma).Map(Enumerable.AsEnumerable);


        public static RWS<MonoidW, R, W, S, Seq<B>> traverse<MonoidW, R, W, S, A, B>(Seq<RWS<MonoidW, R, W, S, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            SeqRwsExtensions.TraverseFast(ma, f).Map(Prelude.Seq);

        public static RWS<MonoidW, R, W, S, Lst<B>> traverse<MonoidW, R, W, S, A, B>(Lst<RWS<MonoidW, R, W, S, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            SeqRwsExtensions.TraverseFast(ma, f).Map(toList);

        public static RWS<MonoidW, R, W, S, Arr<B>> traverse<MonoidW, R, W, S, A, B>(Arr<RWS<MonoidW, R, W, S, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            SeqRwsExtensions.TraverseFast(ma, f).Map(toArray);

        public static RWS<MonoidW, R, W, S, B[]> traverse<MonoidW, R, W, S, A, B>(RWS<MonoidW, R, W, S, A>[] ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            SeqRwsExtensions.TraverseFast(ma, f).Map(x => x.ToArray());

        public static RWS<MonoidW, R, W, S, Set<B>> traverse<MonoidW, R, W, S, A, B>(Set<RWS<MonoidW, R, W, S, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            SeqRwsExtensions.TraverseFast(ma, f).Map(toSet);

        public static RWS<MonoidW, R, W, S, HashSet<B>> traverse<MonoidW, R, W, S, A, B>(HashSet<RWS<MonoidW, R, W, S, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            SeqRwsExtensions.TraverseFast(ma, f).Map(toHashSet);

        public static RWS<MonoidW, R, W, S, Stck<B>> traverse<MonoidW, R, W, S, A, B>(Stck<RWS<MonoidW, R, W, S, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            SeqRwsExtensions.TraverseFast(ma, f).Map(toStack);

        public static RWS<MonoidW, R, W, S, IEnumerable<B>> traverse<MonoidW, R, W, S, A, B>(IEnumerable<RWS<MonoidW, R, W, S, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            SeqRwsExtensions.TraverseFast(ma, f).Map(Enumerable.AsEnumerable);
    }
}
