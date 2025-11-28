using System;
using LanguageExt.Traits;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class ApplicativeExtensions
{
    extension<AF, A, B>((Memo<AF, A>, Memo<AF, B>) items) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, R> Apply<R>(Func<A, B, R> f) =>
            f.Map(items.Item1)
             .Apply(items.Item2);
    }

    extension<AF, A, B, C>((Memo<AF, A>, Memo<AF, B>, Memo<AF, C>) items) where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, R> Apply<R>(Func<A, B, C, R> f) =>
            f.Map(items.Item1)
             .Apply(items.Item2)
             .Apply(items.Item3);
    }

    extension<AF, A, B, C, D>((Memo<AF, A>, Memo<AF, B>, Memo<AF, C>, Memo<AF, D>) items) where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, R> Apply<R>(Func<A, B, C, D, R> f) =>
            f.Map(items.Item1)
             .Apply(items.Item2)
             .Apply(items.Item3)
             .Apply(items.Item4);
    }

    extension<AF, A, B, C, D, E>((Memo<AF, A>, Memo<AF, B>, Memo<AF, C>, Memo<AF, D>, Memo<AF, E>) items) where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, R> Apply<R>(Func<A, B, C, D, E, R> f) =>
            f.Map(items.Item1)
             .Apply(items.Item2)
             .Apply(items.Item3)
             .Apply(items.Item4)
             .Apply(items.Item5);
    }

    extension<AF, A, B, C, D, E, F>((Memo<AF, A>, Memo<AF, B>, Memo<AF, C>, Memo<AF, D>, Memo<AF, E>, Memo<AF, F>) items) where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, R> Apply<R>(Func<A, B, C, D, E, F, R> f) =>
            f.Map(items.Item1)
             .Apply(items.Item2)
             .Apply(items.Item3)
             .Apply(items.Item4)
             .Apply(items.Item5)
             .Apply(items.Item6);
    }

    extension<AF, A, B, C, D, E, F, G>((Memo<AF, A>, Memo<AF, B>, Memo<AF, C>, Memo<AF, D>, Memo<AF, E>, Memo<AF, F>, Memo<AF, G>) items) where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, R> Apply<R>(Func<A, B, C, D, E, F, G, R> f) =>
            f.Map(items.Item1)
             .Apply(items.Item2)
             .Apply(items.Item3)
             .Apply(items.Item4)
             .Apply(items.Item5)
             .Apply(items.Item6)
             .Apply(items.Item7);
    }

    extension<AF, A, B, C, D, E, F, G, H>((Memo<AF, A>, Memo<AF, B>, Memo<AF, C>, Memo<AF, D>, Memo<AF, E>, Memo<AF, F>, Memo<AF, G>, Memo<AF, H>) items) where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, R> Apply<R>(Func<A, B, C, D, E, F, G, H, R> f) =>
            f.Map(items.Item1)
             .Apply(items.Item2)
             .Apply(items.Item3)
             .Apply(items.Item4)
             .Apply(items.Item5)
             .Apply(items.Item6)
             .Apply(items.Item7)
             .Apply(items.Item8);
    }

    extension<AF, A, B, C, D, E, F, G, H, I>((Memo<AF, A>,
                                                 Memo<AF, B>,
                                                 Memo<AF, C>,
                                                 Memo<AF, D>,
                                                 Memo<AF, E>,
                                                 Memo<AF, F>,
                                                 Memo<AF, G>,
                                                 Memo<AF, H>,
                                                 Memo<AF, I>) items) where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, R> Apply<R>(Func<A, B, C, D, E, F, G, H, I, R> f) =>
            f.Map(items.Item1)
             .Apply(items.Item2)
             .Apply(items.Item3)
             .Apply(items.Item4)
             .Apply(items.Item5)
             .Apply(items.Item6)
             .Apply(items.Item7)
             .Apply(items.Item8)
             .Apply(items.Item9);
    }

    extension<AF, A, B, C, D, E, F, G, H, I, J>((Memo<AF, A>,
                                                    Memo<AF, B>,
                                                    Memo<AF, C>,
                                                    Memo<AF, D>,
                                                    Memo<AF, E>,
                                                    Memo<AF, F>,
                                                    Memo<AF, G>,
                                                    Memo<AF, H>,
                                                    Memo<AF, I>,
                                                    Memo<AF, J>) items) where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, R> Apply<R>(Func<A, B, C, D, E, F, G, H, I, J, R> f) =>
            f.Map(items.Item1)
             .Apply(items.Item2)
             .Apply(items.Item3)
             .Apply(items.Item4)
             .Apply(items.Item5)
             .Apply(items.Item6)
             .Apply(items.Item7)
             .Apply(items.Item8)
             .Apply(items.Item9)
             .Apply(items.Item10);
    }
}
